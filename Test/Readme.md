# General setup
The tests have two parts:
- Setup
- Cases

All the *Samples* have their own folder with *Setup* and *Cases*. They even have their own namespaces so that all the dependencies could be localised. Several setups look very similar, but comments are provided to explain what those setups are for.

Stack used
- XUnit
- AutoFixture (With XUnit2 extension)