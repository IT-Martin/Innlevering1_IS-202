import json
import os
from pathlib import Path

root = Path(__file__).resolve().parent.parent
appsettings = json.loads((root / "Innlevering1_Gruppe2" / "appsettings.json").read_text())
config = root / "Innlevering1_Gruppe2" / "appsettings.Local.json"
local_settings = {
    "ConnectionStrings": appsettings["ConnectionStrings"],
    "LocalPreview": True,
}

with os.fdopen(os.open(config, os.O_WRONLY | os.O_CREAT | os.O_EXCL, 0o600), "w") as file:
    json.dump(local_settings, file, indent=2)
    file.write("\n")

print("Local settings created.")
