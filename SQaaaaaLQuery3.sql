
CREATE VIEW ParentReservation_View AS 
	SELECT 'AAA001' AS IntegratedReservationNo, NULL AS ReservationNo, NULL AS BusinessDate, 'CommentC' AS Comment
	UNION
	SELECT 'AAA002' AS IntegratedReservationNo, NULL AS ReservationNo, NULL AS BusinessDate, 'CommentX' AS Comment
GO

CREATE VIEW ChildReservation_View AS 
	SELECT 'AAA001' AS IntegratedReservationNo,'0001' AS ReservationNo, '20221001' AS BusinessDate, 'CommentA' AS Comment
	UNION
	SELECT 'AAA001','0002', '20221001', 'CommentC'
	UNION
	SELECT 'AAA001','0003', '20221002', 'CommentB'
	UNION
	SELECT 'AAA001','0004', '20221002', 'CommentBB'
	UNION
	SELECT 'AAA001','0005', '20221003', 'CommentD'
	UNION
	SELECT 'AAA001','0006', '20221003', 'CommentD'
	UNION
	SELECT 'AAA002','0001', '20221001', 'CommentA'
	UNION
	SELECT 'AAA002','0002', '20221001', 'CommentB'
	UNION
	SELECT 'AAA002','0003', '20221002', 'CommentC'
	UNION
	SELECT 'AAA002','0004', '20221003', 'CommentD'
GO

WITH DuplicateParent AS (
	SELECT
		[Parent].[IntegratedReservationNo],
		[Parent].[Comment]
	FROM
		[ParentReservation_View] AS [Parent]
		JOIN (
			SELECT
				[Parent].[IntegratedReservationNo]
			FROM 
				[ChildReservation_View] AS [Child]
				LEFT JOIN [ParentReservation_View] AS [Parent]
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
						ChildReservation_View AS Child
					) AS t2
				WHERE
					t2.IntegratedReservationNo = t1.IntegratedReservationNo
					AND t2.BusinessDate = t1.BusinessDate
					AND t2.rk = 1
				ORDER BY
					t2.ReservationNo FOR XML PATH('')
			))
		FROM
			ChildReservation_View AS t1
	) AS GroupedChild
	LEFT JOIN DuplicateParent
	ON GroupedChild.IntegratedReservationNo = DuplicateParent.IntegratedReservationNo
)
SELECT * 
FROM Result;


DROP VIEW IF EXISTS ParentReservation_View;
DROP VIEW IF EXISTS ChildReservation_View;


