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
# Folder "Inputs/" stored in variable inputFolder
var inputFolder = Inputs/;

# Creates the file ./output/Web.config by merging ./Inputs/Web.root.config, Inputs/Web.override.config and Inputs/Nonexisting.config
# Non-absolute file paths are relative to Web.config.recipe's parent folder
# Last file wins
# Non-existing files are ignored
Outputs/ + Web.config = inputFolder + [Web.root.config, Web.override.config, Nonexisting.config];
```

### Merge config files on build
Add a pre build event to your project:
path\to\ConfigMerge.exe -recipe:"$(ProjectDir)Web.config.recipe"


