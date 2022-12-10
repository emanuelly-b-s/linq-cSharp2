using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using static System.Console;
using System.Collections.Generic;

var days = getDays();
var bikes = getSharings();

// TODO

IEnumerable<DayInfo> getDays()
{
    StreamReader reader = new StreamReader("dayInfo.csv");
    reader.ReadLine();

    while (!reader.EndOfStream)
    {
        var data = reader.ReadLine().Split(',');
        DayInfo info = new DayInfo();

        info.Day = int.Parse(data[0]);
        info.Season = int.Parse(data[1]);
        info.IsWorkingDay = int.Parse(data[2]) == 1;
        info.Weather = int.Parse(data[3]);
        info.Temp = float.Parse(data[4].Replace('.', ','));

        yield return info;
    }

    reader.Close();
}

IEnumerable<BikeSharing> getSharings()
{
    StreamReader reader = new StreamReader("bikeSharing.csv");
    reader.ReadLine();

    while (!reader.EndOfStream)
    {
        var data = reader.ReadLine().Split(',');
        BikeSharing infoB = new BikeSharing();

        infoB.Day = int.Parse(data[0]);
        infoB.Casual = int.Parse(data[1]);
        infoB.Registred = int.Parse(data[2]);

        yield return infoB;
    }

    reader.Close();

}

//^------------------------------------------------------------------------------------------------------

// Qual a média de alugueis de bicicletas em todo período? Sempre 
// considere os aluguéis casuais junto aos registrados.
var mediaTotalBike = bikes.Average(b => b.Casual + b.Registred);
WriteLine($"Media total de bikes alugadas no periodo total: {mediaTotalBike:0.##}");

//^------------------------------------------------------------------------------------------------------

// 2. A empresa parece ter crescido, ou seja, aumentado os alugueis de 
// cicletas ao longo do tempo? Utilize as médias a cada 30 dias para 
// analisar isso. Dica: Você pode resolver isso com um GroupBy com 
// uma divisão

var media30D = bikes.GroupBy(d => d.Day / 30)
                    .Select(g => new
                    {
                        periodo = g.Key,
                        mediaP = g.Average(b => b.Casual + b.Registred)
                    });


foreach (var item in media30D)
{
    WriteLine($"Periodo {item.periodo} - Media {item.mediaP:0.##}");
}

// 3. Como a estação, condições de tempo e temperatura impactam nos 
// resultados? Responda para os três casos separadamente.

var resultados = bikes.Join(days,
                            b => b.Day,
                            d => d.Day,
                            (b, d) => new
                            {
                                casualDay = b.Casual,
                                registredDay = b.Registred,
                                seasonDay = d.Season,
                                weathersitDay = d.Weather,
                                temp = d.Temp
                            });

var season = resultados.GroupBy(g => g.seasonDay)
                        .Select(s => new
                        {
                            seasonG = s.Key,
                            mediaTb = s.Average(a => a.casualDay + a.registredDay)
                        });

var weathersit = resultados.GroupBy(g => g.weathersitDay)
                        .Select(w => new
                        {
                            wG = w.Key,
                            mediaTb = w.Average(a => a.casualDay + a.registredDay)
                        });

var temp = resultados.GroupBy(g => g.temp)
                    .Select(t => new
                    {
                        t = t.Key,
                        mediaTb = t.Average(a => a.casualDay + a.registredDay)
                    })
                    .OrderByDescending(temp => temp.t);

foreach (var item in season)
{
    WriteLine($"Estação {item.seasonG} - {item.mediaTb:0.##} ");
}

foreach (var item in weathersit)
{
    WriteLine($"Condição do tempo {item.wG} - {item.mediaTb:0.##} ");
}

foreach (var item in temp)
{
    WriteLine($"Temperatura {item.t:0.##} - {item.mediaTb:0.##} ");
}




