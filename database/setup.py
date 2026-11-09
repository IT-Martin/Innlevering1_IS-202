import json
import os
import secrets
import subprocess
from pathlib import Path

root = Path(__file__).resolve().parent.parent
env = root / ".env"
config = root / "Reachout" / "appsettings.Local.json"


def write_private_file(path, content):
    with os.fdopen(os.open(path, os.O_WRONLY | os.O_CREAT | os.O_EXCL, 0o600), "w") as file:
        file.write(content)


if not env.exists():
    result = subprocess.run(
        ["docker", "volume", "ls", "--filter", "label=com.docker.compose.project=reachout-team", "--format", "{{.Name}}"],
        capture_output=True, text=True, check=True,
    )
    if "reachout-team_database" in result.stdout.splitlines():
        raise SystemExit("Fant et eksisterende databasevolum. Gjenopprett den tilhørende .env-filen.")
    write_private_file(env, (
        f"DB_PASSWORD={secrets.token_hex(24)}\n"
        f"ROOT_PASSWORD={secrets.token_hex(24)}\n"
        f"REACHOUT_ADMIN_PASSWORD={secrets.token_hex(18)}\n"
        "REACHOUT_ADMIN_EMAIL=systemansvarlig@reachout.test\n"
        "REACHOUT_DB_PORT=3316\n"
        "REACHOUT_WEB_PORT=5205\n"
    ))

values = dict(line.strip().split("=", 1) for line in env.read_text().splitlines()
              if "=" in line and not line.lstrip().startswith("#"))
if not config.exists():
    write_private_file(config, json.dumps({
        "ConnectionStrings": {
            "DefaultConnection": (
                f"Server=127.0.0.1;Port={values['REACHOUT_DB_PORT']};"
                f"Database=reachout_database;User ID=gruppe2_app;Password={values['DB_PASSWORD']};"
            )
        },
        "LocalPreview": True,
        "InitialAdmin": {
            "Email": values["REACHOUT_ADMIN_EMAIL"],
            "Password": values["REACHOUT_ADMIN_PASSWORD"],
        },
    }, indent=2) + "\n")
print("Lokalt oppsett klart. Kjør docker compose up --build -d.")
