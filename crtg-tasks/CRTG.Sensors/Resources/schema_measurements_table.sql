create table if not exists measurements
(
    sensor_id           INTEGER,
    measurement_time	INTEGER,
	value				NUMERIC,
	collection_time_ms	INTEGER
)