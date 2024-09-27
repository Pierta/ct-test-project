# ct-test-project
## Test project for a tech interview

* Weather API: https://www.weatherapi.com/
* OS: KUbuntu 24.04
* Platform: .NET 8
* Additional components: Redis, Aspire dashboard

Prerequisites:
* Git
* Docker
* Docker compose
* Put your API KEY to `.env` file

Run
```shell
./run.sh
```

Integration tests
```shell
pushd HttpTests
./run-tests.sh
popd
```

Then
* go to http://localhost:8080/swagger/index.html to test the API
* go to http://0.0.0.0:18888/login?t={dashboard_token} to see logs/metrics/traces
  * `dashboard_token` can be found in `open-telemetry-dashboard` container logs

Rebuild (if some code is adjusted)
```shell
./rebuild.sh
```

Clean up
```shell
./clean-up.sh
```