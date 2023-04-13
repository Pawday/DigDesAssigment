--- 1. Запрос на поиск сотрудника с максимальной заработной платой
SELECT * FROM "EMPLOYEE" ORDER BY "SALARY" DESC LIMIT 1;

--- 2. Запрос на максимальную длину цепочки руководителей по таблице сотрудников
WITH RECURSIVE R AS
(
    SELECT
        1 AS "STEP",
        (WITH NOT_LEAD_EMPLOYEES AS (
            SELECT "ID" FROM "EMPLOYEE" WHERE "ID" NOT IN (SELECT "CHIEF_ID" FROM "EMPLOYEE"))
                SELECT array(SELECT "ID" FROM NOT_LEAD_EMPLOYEES)) AS "CURRENT_EMPLOYEE_ARRAY",
        (SELECT array(SELECT "ID" FROM "EMPLOYEE" WHERE "ID" = "CHIEF_ID")) AS "TOP_LEADERS_ARRAY"
    UNION
    SELECT
        "STEP" + 1,
        (WITH NEWSET AS ((SELECT unnest("CURRENT_EMPLOYEE_ARRAY") AS ELEM))
            SELECT array(SELECT DISTINCT (SELECT "CHIEF_ID" FROM "EMPLOYEE" WHERE "ID" = ELEM) FROM NEWSET)),
        "TOP_LEADERS_ARRAY"
    FROM R
    WHERE
        NOT
        (
            array_length("CURRENT_EMPLOYEE_ARRAY", 1) < array_length("TOP_LEADERS_ARRAY", 1)
            OR
            ("CURRENT_EMPLOYEE_ARRAY" = "TOP_LEADERS_ARRAY")
        )
--         AND "STEP" < SOME_MAX_DEPTH_LIMIT --- Где SOME_MAX_DEPTH_LIMIT - опциональный предел цепочки начальников
)
-- SELECT * FROM R;
SELECT "STEP" AS "TREE_DEPTH" FROM R ORDER BY "STEP" DESC LIMIT 1; -- закомментируйте эту строчку и раскомментируйте строчку выше для отображения промежуточных этапов работы запроса



--- 3. Запрос на получение отдела, с максимальной суммарной зарплатой сотрудников
WITH RECURSIVE R AS
(
    WITH DEPATMENTS AS (SELECT "ID" FROM "DEPARTMENT") SELECT array(SELECT "ID" FROM DEPATMENTS) AS "ALL_DEPARTMENTS",
    1 AS "DEPARTMENT_INDEX",
    (SELECT "ID" FROM "DEPARTMENT" LIMIT 1 OFFSET 0) AS "DEPARTMENT_ID",
    (SELECT sum("SALARY") FROM "EMPLOYEE" WHERE "DEPARTMENT_ID" IN ((SELECT "ID" FROM "DEPARTMENT" LIMIT 1 OFFSET 0))) AS "DEP_SALARY_SUM"
    UNION
    SELECT
        "ALL_DEPARTMENTS",
        "DEPARTMENT_INDEX"+1 AS "DEPARTMENT_INDEX",
        (SELECT "ID" FROM "DEPARTMENT" LIMIT 1 OFFSET "DEPARTMENT_INDEX") AS "DEP_ID",
        (SELECT sum("SALARY") FROM "EMPLOYEE" WHERE "DEPARTMENT_ID" IN ((SELECT "ID" FROM "DEPARTMENT" LIMIT 1 OFFSET "DEPARTMENT_INDEX"))) AS "DEP_SALARY_SUM"
    FROM R
    WHERE "DEPARTMENT_INDEX" < array_length("ALL_DEPARTMENTS", 1)
)
-- SELECT * FROM R; --- Все промежуточные результаты работы запроса
-- SELECT "DEPARTMENT_ID" FROM R ORDER BY "DEP_SALARY_SUM" DESC LIMIT 1; --- ID департамента с максимальной суммарной зарплатой сотрудников
SELECT * FROM "DEPARTMENT" WHERE "ID" = (SELECT "DEPARTMENT_ID" FROM R ORDER BY "DEP_SALARY_SUM" DESC LIMIT 1);



--- 4. Запрос сотрудника, чье имя начинается на «Р» и заканчивается на «н».
SELECT * FROM "EMPLOYEE" WHERE "NAME" ~ '^Р.*н$' LIMIT 1;