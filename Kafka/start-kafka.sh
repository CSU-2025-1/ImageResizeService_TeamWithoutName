#!/bin/bash
set -e

# Проверяем, отформатировано ли хранилище
if [ ! -f /opt/kafka/data/.kafka_formatted ]; then
  echo "Formatting Kafka storage..."
  /opt/kafka/bin/kafka-storage.sh format --cluster-id "${CLUSTER_ID}" --config /opt/kafka/config/kraft/server.properties
  if [ $? -eq 0 ]; then
    echo "Kafka storage formatted successfully."
    touch /opt/kafka/data/.kafka_formatted # Создаем файл-индикатор
  else
    echo "ERROR: Kafka storage format FAILED!"
    exit 1
  fi
else
  echo "Kafka storage already formatted."
fi

echo "Starting Kafka server..."
/opt/kafka/bin/kafka-server-start.sh /opt/kafka/config/kraft/server.properties &
echo "Kafka started in background"

# Keep the container running
tail -f /dev/null
