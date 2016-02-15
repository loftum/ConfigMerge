# ConfigMerge
Web/App.config merge tool based on https://github.com/andreakn/ConfigStitcher

---
ConfigMerge lets you merge multiple config files without the need of xdt transforms.

The transformation is specified like this:
```
outputFile = [inputfile1, inputfile2, ...];
```


### Simple usage (commandline argument):
```
ConfigMerge.exe -recipe:App.config=[App.root.config,App.override.config];
```


### Typical usage (recipe file):
ConfigMerge.exe -recipe:Web.config.recipe

#### Web.config.recipe example:
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
path\to\ConfigMerge.exe -recipe:"$(ProjectDir)Web.config.recipe"


