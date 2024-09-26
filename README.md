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
then
* go to http://localhost:8080/swagger/index.html to test the API
* go to http://0.0.0.0:18888/login?t={dashboard_token} to see metrics/traces
  * `dashboard_token` can be found in `open-telemetry-dashboard` container logs

Rebuild
```shell
./rebuild.sh
```

Clean up
```shell
./clean-up.sh
```