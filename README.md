
## Solución NET de demonio

#### Configurar archivo daemon.json
```json
{
	"Name": "DaemonNET",
	"Cron": "0/1 9-19 * * 1-5",
	"RouteCallback": {
		"Host": "http://localhost:3000/ExecuteService"
	}
}
```

"Cron": Cron expression (Ejecuta cada 1 minuto, entre las 00:09 AM y las 19:00 PM, entre los días Lunes y Viernes)

"RouteCallback": URL que llamara mediante HttpClient, envia un parametro en Query "execution=yyyy-MM-ddTHH:mm:ss"
