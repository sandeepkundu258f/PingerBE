# PingerBE

dotnet tool install --global dotnet-ef

echo 'export PATH="$PATH:$HOME/.dotnet/tools"' >> ~/.bashrc

dotnet ef migrations add InitialCreate --project Pinger.Infrastructure --startup-project Pinger.Api -o Persistence/Migrations

dotnet ef database update --project Pinger.Infrastructure --startup-project Pinger.Api

///////////////////
REMOVE
dotnet ef migrations remove --project Pinger.Infrastructure --startup-project Pinger.Api