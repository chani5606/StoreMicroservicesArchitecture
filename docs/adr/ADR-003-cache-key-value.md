# ADR-003: Redis key-value cache למסלול הקריאה של ProductCatalog

- סטטוס: מאושר
- תאריך: 2026-07-06

## הקשר

קריאות לקטלוג הן תכופות ובדרך כלל חוזרות. קריאות ישירות למסד בכל בקשה מעלות latency ועומס על ה-backend.

## החלטה

שימוש ב-Redis כ-cache מבוזר עם תבנית cache-aside ב-ProductCatalogService.

## נימוק

- מודל key-value מתאים במיוחד לחיפוש מהיר לפי מזהה מוצר (`product:{id}`).
- cache-aside שומר את source-of-truth ב-MongoDB ובו בזמן מקטין latency בקריאה.
- invalidation ב-create/update/delete שומרת על מעטפת נכונות טובה.

## מסגרת CAP / עקביות

- שכבת ה-cache מכוונת ל-BASE ולמעשה eventual consistency ביחס ל-DB המקורי.
- המערכת מקבלת סטייה זמנית שמוגבלת על ידי invalidation וזמן תפוגה.

## השלכות

חיובי:
- זמני תגובה נמוכים יותר לקריאות חוזרות.
- פחות עומס על MongoDB תחת תעבורת קריאה גבוהה.

שלילי:
- נדרשת משמעת explicit invalidation.
- ייתכנו קריאות מיושנות אם invalidation נכשלת.
