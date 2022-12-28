WITH Parent AS (
	SELECT 'AAA001' AS IntegratedReservationNo, NULL AS ReservationNo, NULL AS BusinessDate, 'CommentC' AS Comment
	UNION
	SELECT 'AAA002' AS IntegratedReservationNo, NULL AS ReservationNo, NULL AS BusinessDate, 'CommentX' AS Comment
)
,Child AS (
	SELECT 'AAA001' AS IntegratedReservationNo,'0001' AS ReservationNo, '20221001' AS BusinessDate, 'CommentA' AS Comment
	UNION
	SELECT 'AAA001','0002', '20221001', 'CommentC'
	UNION
	SELECT 'AAA001','0003', '20221002', 'CommentB'
	UNION
	SELECT 'AAA001','0004', '20221003', 'CommentD'
	UNION
	SELECT 'AAA002','0001', '20221001', 'CommentA'
	UNION
	SELECT 'AAA002','0002', '20221001', 'CommentB'
	UNION
	SELECT 'AAA002','0003', '20221002', 'CommentC'
	UNION
	SELECT 'AAA002','0004', '20221003', 'CommentD'
)
,val1 AS (
	SELECT DISTINCT
		IntegratedReservationNo,
		BusinessDate,
		Comment = LTRIM((
			SELECT
				' ' + Comment
			FROM
				Child t2
			WHERE
				t2.IntegratedReservationNo = t1.IntegratedReservationNo
				AND t2.BusinessDate = t1.BusinessDate
			ORDER BY
				t2.ReservationNo FOR XML PATH('')
		))
	FROM
		Child t1
)
,val2 AS (
	SELECT
		T1.IntegratedReservationNo,
		T1.BusinessDate,
		LTRIM(CONCAT(ISNULL(T2.Comment,''),' ',T1.Comment)) AS Commnet
	FROM val1 AS T1
	LEFT JOIN Parent AS [T2]
	ON T1.IntegratedReservationNo = T2.IntegratedReservationNo
)
SELECT * 
FROM val1

