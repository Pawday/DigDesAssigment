--- Запрос на поиск сотрудника с максимальной заработной платой
SELECT * FROM "EMPLOYEE" ORDER BY "SALARY" DESC LIMIT 1;

--- Запрос на максимальную длину цепочки руководителей по таблице сотрудников

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
) SELECT "STEP" AS "TREE_DEPTH" FROM R ORDER BY "STEP" DESC LIMIT 1;
-- ) SELECT * FROM R; -- закомментируйте эту строчку и раскомментируйте строчку выше для отображения промежуточных этапов работы запроса


