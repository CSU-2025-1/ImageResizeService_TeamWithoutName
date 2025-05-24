#!/bin/bash
set -e

if [ ! -f /initdb/initialized ]; then
  echo "Initializing replica set..."
  sleep 10;

  echo "Running rs.initiate()..."
  mongo --host mongo-primary --eval "
    rs.initiate({
      _id : 'rs0',
      members: [
        { _id : 0, host : 'mongo-primary:27017' },
        { _id : 1, host : 'mongo-secondary1:27017' },
        { _id : 2, host : 'mongo-secondary2:27017' }
      ]
    });
  "
  echo "rs.initiate() completed."

  echo "Running rs.status()..."
  mongo --host mongo-primary --eval "rs.status()"
  echo "rs.status() completed."

  touch /initdb/initialized
  echo "Replica set initialized successfully."
else
  echo "Replica set already initialized."
fi