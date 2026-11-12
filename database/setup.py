import json
import os
import secrets
import subprocess
from pathlib import Path

root = Path(__file__).resolve().parent.parent
env = root / ".env"
config = root / "Innlevering1_Gruppe2" / "appsettings.Local.json"

defaults = {
    "DB_PASSWORD": lambda: secrets.token_hex(24),
    "ROOT_PASSWORD": lambda: secrets.token_hex(24),
    "REACHOUT_ADMIN_PASSWORD": lambda: secrets.token_hex(18),
    "REACHOUT_ADMIN_EMAIL": lambda: "systemansvarlig@reachout.test",
    "REACHOUT_PUBLIC_ACTOR_PASSWORD": lambda: secrets.token_hex(18),
    "REACHOUT_PUBLIC_ACTOR_EMAIL": lambda: "offentlig@reachout.test",
    "REACHOUT_DB_PORT": lambda: "3316",
    "REACHOUT_WEB_PORT": lambda: "5205",
}


def write_private_file(path, content, append=False):
    flags = os.O_WRONLY | os.O_CREAT | (os.O_APPEND if append else os.O_EXCL)
    with os.fdopen(os.open(path, flags, 0o600), "w") as file:
        file.write(content)


if not env.exists():
    result = subprocess.run(
        [
            "docker",
            "volume",
            "ls",
            "--filter",
            "label=com.docker.compose.project=reachout-team",
            "--format",
            "{{.Name}}",
        ],
        capture_output=True,
        text=True,
        check=True,
    )
    if "reachout-team_database" in result.stdout.splitlines():
        raise SystemExit("An existing database volume was found. Restore its matching .env file.")
    write_private_file(env, "")

env_content = env.read_text()
values = {}
for line in env_content.splitlines():
    if "=" in line and not line.lstrip().startswith("#"):
        key, value = line.strip().split("=", 1)
        values[key] = value

missing = {key: create_value() for key, create_value in defaults.items() if not values.get(key)}
if missing:
    addition = "".join(f"{key}={value}\n" for key, value in missing.items())
    if env_content and not env_content.endswith(("\n", "\r")):
        addition = "\n" + addition
    write_private_file(env, addition, append=True)
    values.update(missing)

if not config.exists():
    write_private_file(
        config,
        json.dumps(
            {
                "ConnectionStrings": {
                    "DefaultConnection": (
                        f"Server=127.0.0.1;Port={values['REACHOUT_DB_PORT']};"
                        f"Database=reachout_database;User ID=gruppe2_app;"
                        f"Password={values['DB_PASSWORD']};"
                    )
                },
                "LocalPreview": True,
                "InitialAdmin": {
                    "Email": values["REACHOUT_ADMIN_EMAIL"],
                    "Password": values["REACHOUT_ADMIN_PASSWORD"],
                },
                "InitialPublicActor": {
                    "Email": values["REACHOUT_PUBLIC_ACTOR_EMAIL"],
                    "Password": values["REACHOUT_PUBLIC_ACTOR_PASSWORD"],
                },
            },
            indent=2,
        )
        + "\n",
    )

print("Local setup is ready.")
