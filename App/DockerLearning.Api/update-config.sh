#!/bin/bash

# Print all environment variables at the start for debugging
echo "Environment variables at script start:"
env | sort
echo "----------------------------------------"
# Set default environment to "DEV"
envName=${ENVIRONMENT_NAME:-"DEV"}

# Define paths to configuration files
webConfig="/inetpub/wwwroot/appsettings.json"
webConfigTest="/inetpub/wwwroot/appsettings.qa.json"
webConfigStage="/inetpub/wwwroot/appsettings.stage.json"
webConfigProd="/inetpub/wwwroot/appsettings.prod.json"

nlogConfig="/inetpub/wwwroot/NLog.config"
nlogConfigTest="/inetpub/wwwroot/NLog.qa.config"
nlogConfigStage="/inetpub/wwwroot/NLog.stage.config"
nlogConfigProd="/inetpub/wwwroot/NLog.prod.config"


# Copy configuration files based on environment (ONLY if needed)
if [[ "$envName" == "TEST" ]]; then
    cp "$webConfigTest" "$webConfig"
    cp "$nlogConfigTest" "$nlogConfig"
elif [[ "$envName" == "STAGE" ]]; then
    cp "$webConfigStage" "$webConfig"
    cp "$nlogConfigStage" "$nlogConfig"
elif [[ "$envName" == "PROD" ]]; then
    cp "$webConfigProd" "$webConfig"
    cp "$nlogConfigProd" "$nlogConfig"
fi  # No else or default case needed

echo "Configuration files have been updated for the $envName environment."

# Replace appsettings and NLog configuration from environment variables
for var in $(env); do
    key=$(echo "$var" | cut -d= -f1)
    value=$(echo "$var" | cut -d= -f2-)

    if [[ "$key" == APPSETTING_* ]]; then
        key_name=${key#APPSETTING_}
        jq --arg key "$key_name" --arg value "$value" '.AppSettings[$key] = $value' "$webConfig" > tmp.$$.json && mv tmp.$$.json "$webConfig"
    elif [[ "$key" == CONNSTR_* ]]; then
        key_name=${key#CONNSTR_}
        jq --arg key "$key_name" --arg value "$value" '.ConnectionStrings[$key] = $value' "$webConfig" > tmp.$$.json && mv tmp.$$.json "$webConfig"
    elif [[ "$key" == NLOGTARGET_* ]]; then
        key_temp=${key#NLOGTARGET_}
        target_name=$(echo "$key_temp" | cut -d '_' -f1)
        key_name=$(echo "$key_temp" | cut -d '_' -f2)
        jq --arg target "$target_name" --arg key "$key_name" --arg value "$value" '.configuration.nlog.targets.target[] | select(.name == $target) | .[$key] = $value' "$nlogConfig" > tmp.$$.xml && mv tmp.$$.xml "$nlogConfig"
    fi
done