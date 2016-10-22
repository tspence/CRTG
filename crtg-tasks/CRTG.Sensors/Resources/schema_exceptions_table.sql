create table if not exists exceptions
(
    sensor_id           INTEGER,
    exception_time		INTEGER,
	exception_text		TEXT NULL,
	stacktrace			TEXT NULL,
	cleared             INTEGER
)