create index if not exists ix_exceptions on exceptions
(
	sensor_id,
	exception_time
)