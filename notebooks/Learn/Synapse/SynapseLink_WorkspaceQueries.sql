-- Full fidelity
SELECT top 10 *
FROM OPENROWSET( 
       'CosmosDB',
       'Account=cosmicgbbcdb-sql-slff;Database=SharedDB;Key=AccountKeyFF',
       Sensor) as docs
ORDER BY id ASC;

-- Well defined
SELECT top 10 *
FROM OPENROWSET( 
       'CosmosDB',
       'Account=cosmicgbbcdb-sql-slwd;Database=SharedDB;Key=AccountKeyWD',
       Sensor) as docs
ORDER BY id ASC;

/***********    VARCHAR(MAX)   *************/
-- Well defined fails without VARCHAR(MAX) definition
-- Filler LEN = 172535
SELECT top 10 *, LEN(Filler) AS FillerLen
FROM OPENROWSET( 
       'CosmosDB',
       'Account=cosmicgbbcdb-sql-slwd;Database=SharedDB;Key=AccountKeyWD',
       Sensor)
WITH (id VARCHAR(10)
, Filler VARCHAR(MAX)) AS Sensor

-- Full fidelity doesn't throw error without VARCHAR(MAX)
-- FILLER LEN = 188450 (string: and /" added)
SELECT top 10 *, LEN(Filler) AS FillerLen
FROM OPENROWSET( 
       'CosmosDB',
       'Account=cosmicgbbcdb-sql-slff;Database=SharedDB;Key=AccountKeyFF',
       Sensor) as docs
ORDER BY id ASC;

-- Full fidelity even with VARCHAR(MAX)
-- FILLER LEN = 188450 (string: and /" added)
SELECT top 10 *, LEN(Filler) AS FillerLen
FROM OPENROWSET( 
       'CosmosDB',
       'Account=cosmicgbbcdb-sql-slff;Database=SharedDB;Key=AccountKeyFF',
       Sensor)
WITH (id VARCHAR(100)
, Filler VARCHAR(MAX)) AS Sensor

-- CURIOSITY: CSV output was truncated to 32KB4

-- TO REVIEW:
-- Even after document has been deleted from container, well defined query without VARCHAR(MAX) fails.
-- Which means once a document with 8000+ characters is inserted, well defined will require WITH clause?
SELECT top 10 *
FROM OPENROWSET( 
       'CosmosDB',
       'Account=cosmicgbbcdb-sql-slwd;Database=SharedDB;Key=AccountKeyWD',
       Sensor) AS Sensor
WHERE id = '1' 

/***********    1000+ Properties   *************/
-- Well defined was able to show up to Metric988, Metric 989 onward has NULLs
-- 800 properties went well and were represented
SELECT top 10 *
FROM OPENROWSET( 
       'CosmosDB',
       'Account=cosmicgbbcdb-sql-slwd;Database=SharedDB;Key=AccountKeyWD',
       Sensor) --as Sensor
WITH (id VARCHAR(10)
, Metric1 BIGINT
, Metric2 BIGINT
, Metric798 BIGINT
, Metric799 BIGINT
, Metric800 BIGINT
, Metric801 BIGINT
, Metric802 BIGINT
, Metric985 BIGINT
, Metric986 BIGINT
, Metric987 BIGINT
, Metric988 BIGINT
, Metric989 BIGINT
, Metric990 BIGINT
, Metric991 BIGINT
, Metric992 BIGINT
, Filler VARCHAR(MAX)) AS Sensor


-- Full fidelity doesn't show columns above Metric6
-- Seems like a document with more than 1000 properties didn't cause new columns to be materialized (NOT TRUE -> possible cause are nested elements in Filler cause limit to be reached)
-- Document id=7 still shows in the output, but only up to Metric5 (this seems a bug, as Metric6 already existed in the list and shows as an empty element in the output)
SELECT top 10 *
FROM OPENROWSET( 
       'CosmosDB',
       'Account=cosmicgbbcdb-sql-slff;Database=SharedDB;Key=AccountKeyFF',
       Sensor) AS Sensor


-- Specifying a bunch of fields
-- Metric7 onwards is empty. Metric6 looks like a bug and needs consistent repro and review.
SELECT top 10 *
FROM OPENROWSET( 
       'CosmosDB',
       'Account=cosmicgbbcdb-sql-slff;Database=SharedDB;Key=AccountKeyFF',
       Sensor)
WITH (id VARCHAR(100)
, Metric1 VARCHAR(100)
, Metric2 VARCHAR(100)
, Metric3 VARCHAR(100)
, Metric4 VARCHAR(100)
, Metric5 VARCHAR(100)
, Metric6 VARCHAR(100)
, Metric7 VARCHAR(100)
, Metric8 VARCHAR(100)
, Metric9 VARCHAR(100)
, Metric10 VARCHAR(100)
-- 800 range
, Metric798 BIGINT
, Metric799 BIGINT
, Metric800 BIGINT
, Metric801 BIGINT
, Metric802 BIGINT
-- 1000 range
, Metric970 VARCHAR(100)
, Metric971 VARCHAR(100)
, Metric972 VARCHAR(100)
, Metric973 VARCHAR(100)
, Metric974 VARCHAR(100)
, Metric975 VARCHAR(100)
, Metric976 VARCHAR(100)
, Metric977 VARCHAR(100)
, Metric978 VARCHAR(100)
, Metric979 VARCHAR(100)
, Metric980 VARCHAR(100)
, Metric981 VARCHAR(100)
, Metric982 VARCHAR(100)
, Metric983 VARCHAR(100)
, Metric984 VARCHAR(100)
, Metric985 VARCHAR(100)
, Metric986 VARCHAR(100)
, Metric987 VARCHAR(100)
, Metric988 VARCHAR(100)
, Metric989 VARCHAR(100)
, Filler VARCHAR(MAX)) AS Sensor


-- Was this behavior caused because of the long string with multiple nested elements?
       -- Seem the answer is YES
-- RETESTING WITH Sensor2

/***********    SENSOR2 Container  *************/
/***********    1000+ Properties   *************/
-- Well defined was able to show up to Metric988, Metric 989 onward has NULLs
-- 800 properties went well and were represented, nothing to add.
SELECT top 10 *
FROM OPENROWSET( 
       'CosmosDB',
       'Account=cosmicgbbcdb-sql-slwd;Database=SharedDB;Key=AccountKeyWD',
       Sensor2) as Sensor

SELECT top 10 *
FROM OPENROWSET( 
       'CosmosDB',
       'Account=cosmicgbbcdb-sql-slwd;Database=SharedDB;Key=AccountKeyWD',
       Sensor2) -- as Sensor
WITH (id VARCHAR(10)
, Metric1 BIGINT
, Metric2 BIGINT
, Metric798 BIGINT
, Metric799 BIGINT
, Metric800 BIGINT
, Metric801 BIGINT
, Metric802 BIGINT
, Metric985 BIGINT
, Metric986 BIGINT
, Metric987 BIGINT
, Metric988 BIGINT
, Metric989 BIGINT
, Metric990 BIGINT
, Metric991 BIGINT
, Metric992 BIGINT
, Filler VARCHAR(MAX)) AS Sensor


-- Full fidelity doesn't show columns above Metric176 - WHY?
SELECT top 10 *
FROM OPENROWSET( 
       'CosmosDB',
       'Account=cosmicgbbcdb-sql-slff;Database=SharedDB;Key=AccountKeyFF',
       Sensor2) AS Sensor

-- Specifying a bunch of fields
-- Metric176 onwards is empty. WHY?
SELECT top 10 *
FROM OPENROWSET( 
       'CosmosDB',
       'Account=cosmicgbbcdb-sql-slff;Database=SharedDB;Key=AccountKeyFF',
       Sensor2)
WITH (id VARCHAR(100)
, Metric1 VARCHAR(100)
, Metric2 VARCHAR(100)
, Metric3 VARCHAR(100)
, Metric4 VARCHAR(100)
, Metric5 VARCHAR(100)
, Metric6 VARCHAR(100)
, Metric7 VARCHAR(100)
, Metric8 VARCHAR(100)
, Metric9 VARCHAR(100)
, Metric100 VARCHAR(100)
, Metric168 VARCHAR(100)
, Metric169 VARCHAR(100)
, Metric170 VARCHAR(100)
, Metric171 VARCHAR(100)
, Metric172 VARCHAR(100)
, Metric173 VARCHAR(100)
, Metric174 VARCHAR(100)
, Metric175 VARCHAR(100)
, Metric176 VARCHAR(100)
, Metric177 VARCHAR(100)
, Metric178 VARCHAR(100)
, Metric180 VARCHAR(100)
, Metric200 VARCHAR(100)
, Metric300 VARCHAR(100)
, Metric400 VARCHAR(100)
, Metric500 VARCHAR(100)
, Metric600 VARCHAR(100)
, Metric700 VARCHAR(100)
-- 800 range
, Metric798 BIGINT
, Metric799 BIGINT
, Metric800 BIGINT
, Metric801 BIGINT
, Metric802 BIGINT
-- 1000 range
, Metric970 VARCHAR(100)
, Metric971 VARCHAR(100)
, Metric972 VARCHAR(100)
, Metric973 VARCHAR(100)
, Metric974 VARCHAR(100)
, Metric975 VARCHAR(100)
, Metric976 VARCHAR(100)
, Metric977 VARCHAR(100)
, Metric978 VARCHAR(100)
, Metric979 VARCHAR(100)
, Metric980 VARCHAR(100)
, Metric981 VARCHAR(100)
, Metric982 VARCHAR(100)
, Metric983 VARCHAR(100)
, Metric984 VARCHAR(100)
, Metric985 VARCHAR(100)
, Metric986 VARCHAR(100)
, Metric987 VARCHAR(100)
, Metric988 VARCHAR(100)
, Metric989 VARCHAR(100)
, Filler VARCHAR(MAX)) AS Sensor

/***********    CHECKING DATA TYPES  *************/
CREATE VIEW vw_CDBWD_Sensor
AS
SELECT *
FROM OPENROWSET( 
       'CosmosDB',
       'Account=cosmicgbbcdb-sql-slwd;Database=SharedDB;Key=AccountKeyWD',
       Sensor) as Sensor
GO

CREATE VIEW vw_CDBFF_Sensor
AS
SELECT *
FROM OPENROWSET( 
       'CosmosDB',
       'Account=cosmicgbbcdb-sql-slff;Database=SharedDB;Key=AccountKeyFF',
       Sensor) AS Sensor       
GO

CREATE VIEW vw_CDBWD_Sensor2
AS
SELECT *
FROM OPENROWSET( 
       'CosmosDB',
       'Account=cosmicgbbcdb-sql-slwd;Database=SharedDB;Key=AccountKeyWD',
       Sensor2) as Sensor
GO

CREATE VIEW vw_CDBFF_Sensor2
AS
SELECT top 10 *
FROM OPENROWSET( 
       'CosmosDB',
       'Account=cosmicgbbcdb-sql-slff;Database=SharedDB;Key=AccountKeyFF',
       Sensor2) AS Sensor       
GO

EXEC sp_describe_first_result_set @tsql="SELECT TOP 0 * FROM vw_CDBFF_Sensor"
EXEC sp_describe_first_result_set @tsql="SELECT TOP 0 * FROM vw_CDBWD_Sensor"
EXEC sp_describe_first_result_set @tsql="SELECT TOP 0 * FROM vw_CDBFF_Sensor2"
EXEC sp_describe_first_result_set @tsql="SELECT TOP 0 * FROM vw_CDBWD_Sensor2"