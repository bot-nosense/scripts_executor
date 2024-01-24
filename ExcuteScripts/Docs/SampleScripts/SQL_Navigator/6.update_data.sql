UPDATE alert_qt10
SET
  delay = SYSTIMESTAMP + 10,
  expiration = 5,
  retry_count = 11
WHERE msgid = (SELECT msgid FROM alert_qt10 WHERE q_name = 'QueueName1' AND ROWNUM = 1);