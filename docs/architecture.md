# מסמך ארכיטקטורה

## 1. הארכיטקטורה הסופית

הפרויקט הזה ממיר מונולית למיקרו־שירותים עם עיבוד Saga אסינכרוני.

### רכיבי ריצה

- ApiGateway (YARP)
- ProductCatalogService
- InventoryService
- OrderService
- NotificationService
- RabbitMQ
- MongoDB
- SQL Server
- Redis
- Seq

### תרשים עליון

```mermaid
flowchart LR
  C[Client] --> G[ApiGateway YARP]
  G --> B[BFF /api/bff/order-details/{orderId}]
  G --> O[OrderService]
  G --> P[ProductCatalogService]
  G --> I[InventoryService]
  G --> N[NotificationService]

  B --> O
  B --> P

  O -->|OrderPlaced| R[(RabbitMQ)]
  R --> I
  I -->|InventoryReserved / InventoryRejected| R
  R --> O
  R --> N

  O --> OS[(SQL Server Order DB)]
  I --> IS[(SQL Server Inventory DB)]
  N --> NS[(SQL Server Notification DB)]
  P --> M[(MongoDB Catalog DB)]
  P --> RC[(Redis Cache)]

  O --> L[(Seq)]
  I --> L
  N --> L
  P --> L
  G --> L
```

## 2. לפני ואחרי

### קו בסיס מונוליתי (ProjectAPI)

API יחיד ב-.NET ומסד SQL אחד עם מודולים צמודים מאוד.

בעיות סקייל צפויות:

1. לא ניתן לבצע סקיילינג עצמאי (קריאות לקטלוג וכתיבות להזמנות גדלות יחד).
2. סיכון ההפצה גבוה (שינוי קטן דורש פריסה מחדש של כל האפליקציה).
3. בידוד תקלות חלש (מסלול חם אחד יכול להוריד את כל המערכת).

### יעד מיקרו־שירותים

- פירוק לפי bounded context
- Database-per-service
- Saga אסינכרוני למחזור החיים של ההזמנה
- גישת לקוחות רק דרך Gateway
- Cache-aside לקריאות של הקטלוג
- לוגים מובנים ומרוכזים

## 3. גבול בין Gateway ל-BFF

### אחריות ה-Gateway

- ניתוב ציבורי לשירותים פנימיים
- איגוד surfaces של שירותים ב-Swagger
- איזון עומסים ברמת endpoint בין מופעי הקטלוג

### אחריות ה-BFF

- עיצוב נתונים לפי לקוח
- נקודת אגרגציה:
  - `GET /api/bff/order-details/{orderId}`
  - מאחדת payload של הזמנה מ-`OrderService` עם payload של מוצר מ-`ProductCatalogService`

### למה BFF לא כשירות נפרד?

- ה-BFF כאן ממומש כ-endpoint בתוך `ApiGateway` ולא כ-container נפרד.
- זה שומר על פריסה פשוטה יותר, אבל עדיין מפריד רעיונית בין routing של Gateway לבין aggregation של BFF.

## 4. תכנון ה-Saga (choreography)

### אירועים

- `OrderPlaced`
- `InventoryReserved`
- `InventoryRejected`

### מסלול תקין

1. OrderService שומר את ההזמנה (`Pending`) ומפרסם `OrderPlaced`.
2. InventoryService שומר מלאי ומפרסם `InventoryReserved`.
3. OrderService מעדכן את ההזמנה ל-`Confirmed`.
4. NotificationService רושם לוג של הודעה ללקוח.

### מסלול פיצוי

1. OrderService מפרסם `OrderPlaced`.
2. InventoryService מזהה מלאי לא מספיק ומפרסם `InventoryRejected`.
3. OrderService מבצע פיצוי על ידי עדכון ההזמנה ל-`Cancelled`.
4. NotificationService רושם לוג של הודעת דחייה.

## 5. אסטרטגיית Cache

- תבנית: cache-aside
- מסלול קריאה:
  - ניסיון לקרוא את Redis key `product:{id}`
  - cache miss -> טעינה מ-MongoDB -> שמירה ב-Redis עם sliding expiration של 10 דקות
- Invalidation:
  - הסרת המפתח ב-create/update/delete

## 6. Correlation ויכולת מעקב

- `CorrelationId` נוצר ונשמר עם כל הזמנה.
- אותו ID עובר בכל חוזי ה-Saga.
- לוגים של הצרכנים כוללים `CorrelationId` בכל מעבר מצב.
- אפשר לשאול ב-Seq או בלוגים של השירותים את כל מסלול ההזמנה לפי ערך קורלציה אחד.

## 7. החלפות טכנולוגיה ונימוק

בפרויקט הזה השתמשתי ב-YARP במקום Ocelot.

למה YARP:
- אינטגרציה טבעית עם ASP.NET Core וביצועים טובים ל-reverse proxy.
- ניתוב גמיש דרך קוד וקונפיגורציה.
- פחות עומס קונספטואלי כשבונים Gateway/BFF פשוטים.
- מאפשר לשלב reverse proxy ו-endpoint custom aggregation באותו host.

החלופה מול Ocelot:
- Ocelot נותן יותר יכולות Gateway מוכנות כברירת מחדל בחבילה אחת.
- YARP נותן שליטה נמוכה יותר וחיבור טוב יותר לצינור של .NET.

לכן כן, השתמשנו ב-YARP בפועל: הוא מיישם את ה-Gateway, ואת ה-BFF בנינו כ-API custom על אותו host.

טכנולוגיית ההודעות נשארה RabbitMQ (הסטאק של הקורס), לכן לא נדרש כאן ניתוח חלופה לברוקר.

## 8. צ'קליסט ראיות

- מסלול תקין של Saga: סטטוס הזמנה `Confirmed` ולוגים מקושרים.
- מסלול פיצוי של Saga: סטטוס הזמנה `Cancelled` ולוגים מקושרים.
- Cache: נלכדו גם `Cache miss` וגם `Cache hit`.
- Load balancing: ערכי `X-Service-Instance` מתחלפים בקריאות חוזרות.
- Health: כל השירותים מסומנים healthy דרך compose.
