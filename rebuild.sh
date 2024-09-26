export GIT_BRANCH_NAME=$(git rev-parse --abbrev-ref HEAD)
export GIT_COMMIT_HASH=$(git rev-parse HEAD)

docker compose -f docker-compose.yml up -d --build weather-forecasting-service
