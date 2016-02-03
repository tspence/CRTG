create index if not exists ix_measurements on measurements
(
    sensor_id,
	measurement_time
)