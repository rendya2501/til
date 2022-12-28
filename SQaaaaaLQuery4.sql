
CREATE TABLE #ParentReservation(
	IntegratedReservationNo NVARCHAR(255),
	ReservationNo  NVARCHAR(255),
	BusinessDate DATETIME,
	Comment NVARCHAR(255),
)


CREATE TABLE #ChildReservation(
	IntegratedReservationNo NVARCHAR(255),
	ReservationNo  NVARCHAR(255),
	BusinessDate DATETIME,
	Comment NVARCHAR(255),
)

INSERT INTO #ParentReservation 
VALUES
	('AAA001',NULL, NULL, 'CommentC'),
	('AAA002', NULL , NULL , 'CommentX' )

INSERT INTO #ChildReservation
VALUES
	('AAA001','0001', '20221001', 'CommentA'),
	('AAA001','0002', '20221001', 'CommentC'),
	('AAA001','0003', '20221002', 'CommentB'),
	('AAA001','0004', '20221002', 'CommentBB'),
	('AAA001','0005', '20221003', 'CommentD'),
	('AAA001','0006', '20221003', 'CommentD'),
	('AAA002','0001', '20221001', 'CommentA'),
	('AAA002','0002', '20221001', 'CommentB'),
	('AAA002','0003', '20221002', 'CommentC'),
	('AAA002','0004', '20221003', 'CommentD')
GO

WITH DuplicateParent AS (
	SELECT
		[Parent].[IntegratedReservationNo],
		[Parent].[Comment]
	FROM
		[#ParentReservation] AS [Parent]
		JOIN (
			SELECT
				[Parent].[IntegratedReservationNo]
			FROM 
				[#ChildReservation] AS [Child]
				LEFT JOIN [#ParentReservation] AS [Parent]
				ON [Child].[IntegratedReservationNo] = [Parent].[IntegratedReservationNo]
				AND [Child].[Comment] = [Parent].[Comment]
			WHERE
				[Parent].[IntegratedReservationNo] IS NOT NULL
		) AS [SQ]
		ON [Parent].[IntegratedReservationNo] <> [SQ].[IntegratedReservationNo]
)
,Result AS (
	SELECT
		GroupedChild.IntegratedReservationNo,
		GroupedChild.BusinessDate,
		LTRIM(CONCAT(ISNULL(DuplicateParent.Comment,''),' ',GroupedChild.Comment)) AS Commnet
	FROM (
		SELECT DISTINCT
			IntegratedReservationNo,
			BusinessDate,
			Comment = LTRIM((
				SELECT
					' ' + Comment
				FROM
					(		
					SELECT 
						Child.*, 
						RANK() OVER(PARTITION BY IntegratedReservationNo,BusinessDate,Comment ORDER BY ReservationNo) AS rk
					FROM
						#ChildReservation AS Child
					) AS t2
				WHERE
					t2.IntegratedReservationNo = t1.IntegratedReservationNo
					AND t2.BusinessDate = t1.BusinessDate
					AND t2.rk = 1
				ORDER BY
					t2.ReservationNo FOR XML PATH('')
			))
		FROM
			#ChildReservation AS t1
	) AS GroupedChild
	LEFT JOIN DuplicateParent
	ON GroupedChild.IntegratedReservationNo = DuplicateParent.IntegratedReservationNo
)
SELECT * 
FROM Result;


