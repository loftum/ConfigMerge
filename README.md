# ConfigMerge
Web/App.config merge tool based on https://github.com/andreakn/ConfigStitcher

---
ConfigMerge lets you merge multiple config files without the need of xdt transforms.

## Example transformation
App.root.config:
```xml
<configuration>
    <appSettings>
        <add key="toBeOverridden" value="should be overridden"/>
        <add key="toBeDeleted" value="should be deleted"/>
        <add key="toBeleftAlone" value="unchanged"/>
    </appSettings>
    <connectionStrings>
        <add name="toBeOverridden" connectionString="should be overridden"/>
        <add name="toBeDeleted" connectionString="should be deleted"/>
        <add name="toBeleftAlone" connectionString="unchanged"/>
    </connectionStrings>
    <system.runtime.caching></system.runtime.caching>
</configuration>
```

App.override.config:
```xml
<configuration>
    <appSettings>
        <add key="added" value="added value"/>
        <add key="toBeOverridden" value="overridden"/>
        <add key="toBeDeleted"  DELETEME="true"/>
    </appSettings>
    <connectionStrings>
        <add name="added" connectionString="added connection string"/>
        <add name="toBeOverridden" connectionString="overridden"/>
        <add name="toBeDeleted" DELETEME="true"/>
    </connectionStrings>
    <assemblyBinding></assemblyBinding>
    <system.runtime.caching DELETEME="true"></system.runtime.caching>
</configuration>
```

Result:
```xml
<configuration>
  <appSettings>
    <add key="toBeOverridden" value="overridden" />
    <add key="toBeleftAlone" value="unchanged" />
    <add key="added" value="added value" />
  </appSettings>
  <connectionStrings>
    <add name="toBeOverridden" connectionString="overridden" />
    <add name="toBeleftAlone" connectionString="unchanged" />
    <add name="added" connectionString="added connection string" />
  </connectionStrings>
  <assemblyBinding></assemblyBinding>
</configuration>
```

## Transformation recipies

A transformation is specified like this:
```
outputFile = [inputfile1, inputfile2, ...];
```

- Last input file wins
- Non-existing files are ignored


### Simple usage (commandline argument):
```
C:\some\path\ConfigMerge.exe -recipe:App.config=[App.root.config,App.override.config];
```

- Relative paths are relative the current directory. In this case: C:\some\path


### Typical usage (recipe file):
Recipies can be specified in a recipe file:

```
ConfigMerge.exe -recipe:Web.config.recipe
```

- Relative paths are relative to the recipe file's parent directory.

#### Example recipe file (Web.config.recipe):
```
# Folder "input/" stored in variable inputFolder
var inputFolder = input/;
var outputFolder = output/;


# Non-absolute file paths are relative to Web.config.recipe's parent folder
# Last file wins
# Non-existing files are ignored

# Create ./output/Web.test.config by merging ./input/Web.root.config, input/Web.test.override.config and input/Web.dev.config
# Web.dev.config might not be tracked by source control
outputFolder + Web.test.config = inputFolder + [Web.root.config, Web.test.override.config, Web.dev.config];

# Create ./output/Web.prod.config by merging ./input/Web.root.config and input/Web.prod.override.config
outputFolder + Web.prod.config = inputFolder + [Web.root.config, Web.prod.override.config];

# Create output based on input/Web.input1.config, input/Web.input2.config, etc.
Web.manyinputs.config = inputFolder + Web. + [input1, input2, input3, input4] + .config;

```

### Merge config files on build
Add a pre build event to your project:
```
path\to\ConfigMerge.exe -recipe:"$(ProjectDir)Web.config.recipe"
```

## Under the hood:
ConfigMerge parses the recipe and creates a lambda expression that is compiled and invoked with a ConfigTransformer. The ConfigTransformer then merges the given inputs, and writes the given outputs.

