# פרויקט הגמר של StoreMicroservices

ריפקטור של תהליך הזמנות למסחר אלקטרוני למבנה של מיקרו־שירותים ברמת ייצור.

## שירותים

- ApiGateway (YARP) - נקודת כניסה ציבורית יחידה בפורט `5000`
- ProductCatalogService - APIs למוצרים/קטגוריות, MongoDB + Redis בתבנית cache-aside
- InventoryService - אירועי שמירת מלאי ודחייה
- OrderService - יצירת הזמנה ומעברי מצב של Saga
- NotificationService - צרכן שמדווח על הסטטוס הסופי

## תשתית

- RabbitMQ (`5672`, ניהול `15672`)
- SQL Server (`14330`)
- MongoDB (`27017`)
- Redis (`6379`)
- Seq לצבירת לוגים (`5341`)

## הרצה בפקודה אחת

מהשורש של הריפו:

```powershell
docker compose --profile lb up -d --build
```

זה מפעיל:
- את כל השירותים
- שני מופעי קטלוג (`productcatalogservice` + `productcatalogservice-lb`) מאחורי איזון עומסים בגייטוויי
- לוגים מרוכזים ב-Seq

## בדיקות בריאות

כל השירותים חושפים `/health` ומחוברים ל-healthchecks ב-`docker compose`.

בדיקה מהירה:

```powershell
docker compose --profile lb ps
```

אמור להופיע `(healthy)` עבור:
- `apigateway`
- `orderservice`
- `inventoryservice`
- `notificationservice`
- `productcatalogservice`
- `productcatalogservice-lb`

## דוגמאות API דרך הגייטוויי בלבד

### 1. עיון במוצרים

```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/products" -Method Get
```

### 2. ביצוע הזמנה - מסלול תקין של Saga

```powershell
$body = @{ legacyId = 2001; userId = 1; productId = 1; quantity = 1; unitPrice = 99.9 } | ConvertTo-Json
Invoke-RestMethod -Uri "http://localhost:5000/api/orders" -Method Post -Body $body -ContentType "application/json"
```

### 3. ביצוע הזמנה - מסלול פיצוי של Saga

```powershell
$body = @{ legacyId = 2002; userId = 1; productId = 1; quantity = 99999; unitPrice = 99.9 } | ConvertTo-Json
Invoke-RestMethod -Uri "http://localhost:5000/api/orders" -Method Post -Body $body -ContentType "application/json"
```

### 4. נקודת BFF (פרטי הזמנה = הזמנה + מוצר)

```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/bff/order-details/1" -Method Get
```

## הוכחת איזון עומסים

קריאות חוזרות מחזירות כותרות אינסטנס שונות:

```powershell
1..8 | ForEach-Object {
  $r = Invoke-WebRequest -Uri "http://localhost:5000/api/products/by-legacy/1" -UseBasicParsing
  [pscustomobject]@{
    Iteration = $_
    Instance = $r.Headers['X-Service-Instance']
    Host = $r.Headers['X-Service-Host']
  }
} | Format-Table -AutoSize
```

## הוכחת Cache hit/miss

קוראים לאותו מוצר פעמיים, ואז בודקים את הלוגים:

```powershell
$id = (Invoke-RestMethod -Uri "http://localhost:5000/api/products" -Method Get)[0].id
Invoke-RestMethod -Uri ("http://localhost:5000/api/products/" + $id) -Method Get | Out-Null
Invoke-RestMethod -Uri ("http://localhost:5000/api/products/" + $id) -Method Get | Out-Null

docker compose --profile lb logs productcatalogservice productcatalogservice-lb --tail 200 | Select-String -Pattern "Cache miss|Cache hit"
```

## הוכחת מעקב לפי Correlation ID

אחרי יצירת הזמנה, לוקחים `correlationId` מהתגובה ומחפשים אותו בלוגים של כל הצרכנים:

```powershell
$cid = "PUT_CORRELATION_ID_HERE"
docker compose logs orderservice inventoryservice notificationservice --tail 300 | Select-String -Pattern $cid
```

## תצפיתיות

- לוגים מובנים: Serilog בכל השירותים
- צבירה: Seq ב-`http://localhost:5341`
- שדות Correlation באירועי ה-Saga ובלוגים של הצרכנים

## תיעוד

- מסמך ארכיטקטורה: `docs/architecture.md`
- מסמכי ADR: `docs/adr/`

## עצירה

```powershell
docker compose --profile lb down
```
