CREATE TABLE "DEPARTMENT"
(
    "ID" SERIAL PRIMARY KEY,
    "NAME" VARCHAR(100)
);

CREATE TABLE "EMPLOYEE"
(
    "ID" SERIAL PRIMARY KEY,
    "DEPARTMENT_ID" SERIAL,
    "CHIEF_ID" SERIAL,
    "NAME" VARCHAR(200),
    "SALARY" NUMERIC,
    FOREIGN KEY("DEPARTMENT_ID") REFERENCES "DEPARTMENT"("ID"),
    FOREIGN KEY("CHIEF_ID") REFERENCES "EMPLOYEE"("ID")
);