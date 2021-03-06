# Licensing and Permitting Dynamics 365 Solutions

You may need to apply to the [Environment Agency](https://www.gov.uk/government/organisations/environment-agency) for an environmental permit if your business uses, recycles, treats, stores or disposes of waste or mining waste. This permit can be for activities at one site or for mobile plant that can be used at many sites.

There are multiple Licensing and Permitting services which will all be based on Dynamics 365. The generic Licensing and Permitting Solution and the Solution for each specific service are contained within this repository.

This service is currently beta for the Waste Permitting Solution and has been developed in accordance with the [Digital by Default service standard](https://www.gov.uk/service-manual/digital-by-default), putting user needs first and delivered iteratively.

## Prerequisites

Please make sure the following are installed:

- Visual Studio 2015
- [Latest version of Dynamics 365 SDK](https://www.microsoft.com/en-us/download/details.aspx?id=50032)
- [Github for Visual Studio](https://visualstudio.github.com/)

## Visual Studio Solutions

The Visual Studio solutions reflect the Dynamics 365 Solutions. Additional Solutions will be added as subsequent Licensing and Permitting services are developed:-

- Core - Common Customer configuration that will be common across all DEFRA CRM instances
- Licensing and Permitting - Generic Licensing and Permitting Configuration
- Waste Permits - Configuration specific to Waste Permits

## Installation

Clone the repository:

```bash
git clone https://github.com/DEFRA/license-and-permitting-dynamics.git && cd license-and-permitting-dynamics
```

Build each solution. All nuget package dependencies will be automatically installed.

Once built, import the Solutions into Dynamics 365 in the following order:-

1) Core
2) Licensing and Permitting
3) Waste Permits

## License

THIS INFORMATION IS LICENSED UNDER THE CONDITIONS OF THE OPEN GOVERNMENT LICENCE found at:

<http://www.nationalarchives.gov.uk/doc/open-government-licence/version/3>

The following attribution statement MUST be cited in your products and applications when using this information.

>Contains public sector information licensed under the Open Government license v3

### About the license

The Open Government Licence (OGL) was developed by the Controller of Her Majesty's Stationery Office (HMSO) to enable information providers in the public sector to license the use and re-use of their information under a common open licence.

It is designed to encourage use and re-use of information freely and flexibly, with only a few conditions.
