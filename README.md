# ct-test-project
## Test project for a tech interview

* Weather API: https://www.weatherapi.com/
* OS: KUbuntu 24.04
* Platform: .NET 8

Prerequisites:
* Git
* Docker
* Docker compose
* Put your API KEY to `.env` file

Run
```shell
./run.sh
```

Rebuild and run
```shell
docker compose -f docker-compose.yml up -d --build weather-forecasting-service
```

Clean up
```shell
./clean-up.sh
```