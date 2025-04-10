FROM apache/kafka:latest

RUN mkdir -p /opt/kafka/data

COPY server.properties /opt/kafka/config/kraft/server.properties
COPY start-kafka.sh /usr/local/bin/start-kafka.sh

USER root 

RUN chmod +x /usr/local/bin/start-kafka.sh

ENV CLUSTER_ID="EzUmKqn9SIiJsVfyvl01SA"
ENV KAFKA_NODE_ID="1"
ENV KAFKA_PROCESS_ROLES="broker,controller"
ENV KAFKA_LISTENERS="PLAINTEXT://:9092,CONTROLLER://:9093,EXTERNAL://:9094"
ENV KAFKA_ADVERTISED_LISTENERS="PLAINTEXT://kafka:9092,CONTROLLER://kafka:9093,EXTERNAL://host.docker.internal:9094"
ENV KAFKA_LISTENER_SECURITY_PROTOCOL_MAP="CONTROLLER:PLAINTEXT,PLAINTEXT:PLAINTEXT,EXTERNAL:PLAINTEXT"
ENV KAFKA_CONTROLLER_QUORUM_VOTERS="1@kafka:9093"
ENV KAFKA_CONTROLLER_LISTENER_NAMES="CONTROLLER"
ENV KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR="1"
ENV KAFKA_TRANSACTION_STATE_LOG_REPLICATION_FACTOR="1"
ENV KAFKA_TRANSACTION_STATE_LOG_MIN_ISR="1"
ENV KAFKA_JMX_PORT="9999"

ENTRYPOINT ["/usr/local/bin/start-kafka.sh"]

HEALTHCHECK --interval=30s --timeout=10s --retries=3 \
  CMD /opt/kafka/bin/kafka-topics.sh --bootstrap-server localhost:9092 --list || exit 1