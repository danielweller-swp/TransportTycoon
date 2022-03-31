# Training

`dotnet run train_model <MODEL-TYPE> <DATA-FILE>`

# Simulation

`dotnet run run_simulation <MODEL-TYPE> <MODEL-FILE> <TEST-DATA-FILE>`

# Parameters

`<MODEL-TYPE>` can be one of `average, linear, polynomial`.

# Examples

```
dotnet run train_model linear s02e04_train.csv
dotnet run run_simulation linear model.json s02e04_test.csv
```
