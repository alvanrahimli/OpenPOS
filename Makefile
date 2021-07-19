PROJS = $(wildcard src/*/*/*.csproj)

define \n


endef

build:
	$(foreach p,$(PROJS),dotnet build $(p)${\n})
