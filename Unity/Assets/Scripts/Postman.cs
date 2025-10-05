using System;
using UnityEngine;
using Random = System.Random;
using StampType = StampModel.StampType;

public class Postman : MonoBehaviour
{
    public static Postman Instance { get; private set; }

    [SerializeField] private Vector2[] envelopeSizes;
    [SerializeField] private Color[] envelopeColors;
    [SerializeField] private Color[] postmarkColors;

    private Random random = new();

    private string[] names =
    {
        "Luca", "Anna", "Johan", "Elena", "Jakub", "Marie", "Nils", "Sofia", "Tomasz", "Katarzyna", "Rafael", "Ivana", "Daniela", "Antoine", "Petra", "Dimitrios", "Giulia", "Zoltan", "Sandra", "Pavel", "Ines", "Mateusz", "Elisabeth", "Viktor", "Carmen", "Lorenzo", "Veronika", "Mihai", "Beatriz", "Jakov", "Natalia", "Ole", "Lucia", "Filip", "Camille", "Álvaro", "Rita", "Maja", "Stefan", "Anna", "Alexandru", "Emilia", "Henrik", "Nikola", "Marina", "David", "Isabelle", "Eero", "Adrian", "Silvia", "Kristof", "Daria", "Leon", "Vera", "Lucas", "Mirela", "Dominik", "Martina", "Tereza", "Jelena", "Joao", "Leonie", "Tobias", "Olga", "Giorgos", "Sara", "Karolina", "Vladimir", "Jean", "Ana", "Maksym", "Erik", "Magdalena", "Rok", "Lucie", "Francesco", "Jasna", "Andrei", "Noemi", "Bastien", "Agnieszka", "Valerio", "Susanna", "Ondrej", "Bianca", "Stefan", "Alba", "Benedikt", "Izabela", "Filippo", "Dominika", "Maxim", "Helena", "Ricardo", "Kristina", "Milan", "Adela", "Enzo", "Tamara", "Olivier", "Alenka"
    };

    private string[] familyNames =
    {
        "Smith", "Miller", "Schneider", "Kovacs", "Novak", "Martin", "Andersen", "Popa", "Nowak", "Garcia", "Brown", "Ivanov", "Petrov", "Silva", "Fischer", "Keller", "Weber", "Huber", "Bauer", "Romanov", "Wagner", "Durand", "Rossi", "Marin", "Nielsen", "Kowalski", "Larsen", "Costa", "Lopez", "Dimitrov", "Santos", "Horvath", "Georgiev", "Farkas", "Molnar", "Sokolov", "Greco", "Pavlov", "Moreau", "Baran", "Adam", "Radu", "Gomez", "Ramos", "Ricci", "Berger", "Kral", "Ferrari", "Mason", "Petrovic", "Mihailov", "Benedek", "Klein", "Gruber", "Toth", "Fontana", "Moreno", "Popescu", "Varga", "Marques", "Jensen", "Rinaldi", "Markov", "Bianchi", "Berardi", "Antonescu", "Vincent", "Ortega", "Castro", "Lombardi", "Voigt", "Caruso", "Vargas", "Gonzalez", "Becker", "Marques", "Pereira", "Schmidt", "Jakobsen", "Franco", "Bianchi", "Salvatore", "Andersson", "Friedman", "Palmer", "Muller", "Schmitt", "Carvalho", "Dupont", "Bond", "Sanders", "Delgado", "Castelli", "Roman", "Bruno", "Griffin", "Ritter", "Rosso", "Leonard", "Fischer", "Robert"
    };

    private string[] streets =
    {
        "Main Street", "Church Street", "Station Road", "High Street", "Park Avenue", "Oak Street", "Maple Street", "Hill Street", "River Road", "School Lane",
        "Bridge Street", "Mill Street", "Lake Road", "Market Street", "Garden Street", "North Road", "South Street", "West Avenue", "East Street", "Central Avenue",
        "King Street", "Queen Street", "Baker Street", "Cross Street", "Sunset Avenue", "Valley Road", "New Street", "Old Street", "Meadow Lane", "Forest Road",
        "Spring Street", "Green Street", "Elm Street", "Willow Road", "Pine Street", "Brook Street", "Field Lane", "Cliff Road", "Harbor Street", "Bay Road",
        "Cherry Street", "Rose Lane", "Cedar Road", "Vine Street", "Birch Road", "Stone Street", "Tower Road", "Canal Street", "Grove Street", "Linden Road",
        "Orchard Lane", "Garden Way", "Hilltop Road", "Cottage Lane", "Abbey Road", "Fox Lane", "Woodland Road", "Heath Road", "Windmill Street", "Beacon Hill",
        "Bridge Lane", "Station Way", "Hunter Street", "Knight Street", "Smith Lane", "Ferry Road", "Harbour Way", "Farm Road", "Manor Lane", "Kingsway",
        "Queensway", "Court Road", "Chapel Street", "Village Road", "Barn Lane", "Ridge Road", "Sunrise Avenue", "Sunset Boulevard", "Valley View", "River View",
        "Coast Road", "Cliffside Drive", "Seaview Road", "Hillcrest Drive", "Northfield Road", "Southview Avenue", "Eastgate", "Westwood Drive", "Field View",
        "Elmwood Drive", "Maple Avenue", "Pine Grove", "Willow Walk", "Brookside", "Millbrook Road", "Meadow View", "Forest View", "Oakwood Drive", "Crescent Road",
        "Park View", "Garden Grove", "Tower Hill", "Lake View", "Rosewood Drive", "Foxhill Road", "Woodside Avenue", "Heathfield Road", "Old Mill Lane"
    };

    private string[] cities =
    {
        "<size=80%>10115<size=100%> Berlin\nGermany",
        "<size=80%>00184<size=100%> Rome\nItaly",
        "<size=80%>08001<size=100%> Barcelona\nSpain",
        "<size=80%>1000<size=100%> Brussels\nBelgium",
        "<size=80%>1010<size=100%> Vienna\nAustria",
        "<size=80%>11000<size=100%> Prague\nCzech Republic",
        "<size=80%>1051<size=100%> Budapest\nHungary",
        "<size=80%>00100<size=100%> Helsinki\nFinland",
        "<size=80%>4051<size=100%> Basel\nSwitzerland",
        "<size=80%>8001<size=100%> Zurich\nSwitzerland",
        "<size=80%>40213<size=100%> Dusseldorf\nGermany",
        "<size=80%>2000<size=100%> Antwerp\nBelgium",
        "<size=80%>1011<size=100%> Amsterdam\nNetherlands",
        "<size=80%>1550<size=100%> Copenhagen\nDenmark",
        "<size=80%>0160<size=100%> Oslo\nNorway",
        "<size=80%>9722<size=100%> Groningen\nNetherlands",
        "<size=80%>5000<size=100%> Odense\nDenmark",
        "<size=80%>5003<size=100%> Bergen\nNorway",
        "<size=80%>11120<size=100%> Stockholm\nSweden",
        "<size=80%>80100<size=100%> Naples\nItaly",
        "<size=80%>10121<size=100%> Turin\nItaly",
        "<size=80%>50123<size=100%> Florence\nItaly",
        "<size=80%>8010<size=100%> Graz\nAustria",
        "<size=80%>20095<size=100%> Hamburg\nGermany",
        "<size=80%>04109<size=100%> Leipzig\nGermany",
        "<size=80%>90402<size=100%> Nuremberg\nGermany",
        "<size=80%>80331<size=100%> Munich\nGermany",
        "<size=80%>14467<size=100%> Potsdam\nGermany",
        "<size=80%>39104<size=100%> Magdeburg\nGermany",
        "<size=80%>28195<size=100%> Bremen\nGermany",
        "<size=80%>18055<size=100%> Rostock\nGermany",
        "<size=80%>01067<size=100%> Dresden\nGermany",
        "<size=80%>50667<size=100%> Cologne\nGermany",
        "<size=80%>68159<size=100%> Mannheim\nGermany",
        "<size=80%>70173<size=100%> Stuttgart\nGermany",
        "<size=80%>91054<size=100%> Erlangen\nGermany",
        "<size=80%>30159<size=100%> Hanover\nGermany",
        "<size=80%>60311<size=100%> Frankfurt\nGermany",
        "<size=80%>55116<size=100%> Mainz\nGermany",
        "<size=80%>86150<size=100%> Augsburg\nGermany",
        "<size=80%>99423<size=100%> Weimar\nGermany",
        "<size=80%>52062<size=100%> Aachen\nGermany",
        "<size=80%>21000<size=100%> Split\nCroatia",
        "<size=80%>10000<size=100%> Zagreb\nCroatia",
        "<size=80%>1000<size=100%> Ljubljana\nSlovenia",
        "<size=80%>81000<size=100%> Podgorica\nMontenegro",
        "<size=80%>1000<size=100%> Skopje\nNorth Macedonia",
        "<size=80%>71000<size=100%> Sarajevo\nBosnia and Herzegovina",
        "<size=80%>7000<size=100%> Chur\nSwitzerland",
        "<size=80%>1201<size=100%> Geneva\nSwitzerland",
        "<size=80%>9000<size=100%> St Gallen\nSwitzerland",
        "<size=80%>6900<size=100%> Lugano\nSwitzerland",
        "<size=80%>1005<size=100%> Lausanne\nSwitzerland",
        "<size=80%>1100<size=100%> Lisbon\nPortugal",
        "<size=80%>4000<size=100%> Porto\nPortugal",
        "<size=80%>2900<size=100%> Setubal\nPortugal",
        "<size=80%>4700<size=100%> Braga\nPortugal",
        "<size=80%>3000<size=100%> Coimbra\nPortugal",
        "<size=80%>10552<size=100%> Athens\nGreece",
        "<size=80%>54621<size=100%> Thessaloniki\nGreece",
        "<size=80%>85100<size=100%> Rhodes\nGreece",
        "<size=80%>24100<size=100%> Kalamata\nGreece",
        "<size=80%>49100<size=100%> Corfu\nGreece",
        "<size=80%>71202<size=100%> Heraklion\nGreece",
        "<size=80%>101<size=100%> Reykjavik\nIceland",
        "<size=80%>100<size=100%> Torshavn\nFaroe Islands",
        "<size=80%>VLT 1110<size=100%> Valletta\nMalta",
        "<size=80%>1010<size=100%> Nicosia\nCyprus",
        "<size=80%>01109<size=100%> Vilnius\nLithuania",
        "<size=80%>4400<size=100%> Kaunas\nLithuania",
        "<size=80%>LV-1050<size=100%> Riga\nLatvia",
        "<size=80%>10111<size=100%> Tallinn\nEstonia",
        "<size=80%>51014<size=100%> Tartu\nEstonia",
        "<size=80%>11000<size=100%> Belgrade\nSerbia",
        "<size=80%>18000<size=100%> Nis\nSerbia",
        "<size=80%>21000<size=100%> Novi Sad\nSerbia",
        "<size=80%>81101<size=100%> Bratislava\nSlovakia",
        "<size=80%>04001<size=100%> Kosice\nSlovakia",
        "<size=80%>1000<size=100%> Luxembourg\nLuxembourg",
        "<size=80%>4002<size=100%> Esch-sur-Alzette\nLuxembourg",
        "<size=80%>AD500<size=100%> Andorra la Vella\nAndorra",
        "<size=80%>47890<size=100%> San Marino\nSan Marino",
        "<size=80%>9490<size=100%> Vaduz\nLiechtenstein",
        "<size=80%>00120<size=100%> Vatican City\nVatican City",
        "<size=80%>98000<size=100%> Monaco\nMonaco"
    };

    public string[] frenchCities =
    {
        "<size=80%>75008<size=100%> Paris\nFrance",
        "<size=80%>69001<size=100%> Lyon\nFrance",
        "<size=80%>31000<size=100%> Toulouse\nFrance",
        "<size=80%>60000<size=100%> Nice\nFrance",
        "<size=80%>59000<size=100%> Lille\nFrance",
        "<size=80%>38000<size=100%> Grenoble\nFrance",
        "<size=80%>35000<size=100%> Rennes\nFrance",
        "<size=80%>21000<size=100%> Dijon\nFrance",
        "<size=80%>17000<size=100%> La Rochelle\nFrance",
        "<size=80%>14000<size=100%> Caen\nFrance",
        "<size=80%>72000<size=100%> Le Mans\nFrance",
        "<size=80%>66000<size=100%> Perpignan\nFrance",
        "<size=80%>30000<size=100%> Nimes\nFrance",
        "<size=80%>33000<size=100%> Bordeaux\nFrance"
    };
    
    private void Awake() => Instance = this;

    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // AddLetter(true);
    }

    public void OnMailboxClick()
    {
        int count = random.Next(3, 5);
        for (int i = 0; i < count; i++)
            AddLetter(i == count - 1);
    }

    public void AddLetter(bool canBeRare)
    {
        var envelopeColor = envelopeColors[random.Next(0, envelopeColors.Length)];
        StampModel stamp = null;
        while (stamp == null)
        {
            stamp = new StampModel((StampType)random.Next(0, Enum.GetValues(typeof(StampType)).Length - 1), (float)random.NextDouble() * 0.25f, (float)random.NextDouble() * 0.75f + 0.25f, new Vector2((float)random.NextDouble() * 0.3f - 0.15f, (float)random.NextDouble() * 0.3f - 0.15f), (float)random.NextDouble() * 60 - 30, postmarkColors[random.Next(0, postmarkColors.Length)], RandomDateUtil.RandomDateTimeExp(random), envelopeColor);
            if (stamp.Rarity > StampModel.StampRarity.Common && !canBeRare)
                stamp = null;
        }

        GameView.Instance.AddLetter(new LetterModel(envelopeSizes[random.Next(0, envelopeSizes.Length)], envelopeColor, $"{names[random.Next(0, names.Length - 1)]} {familyNames[random.Next(0, familyNames.Length - 1)]}\n{streets[random.Next(0, streets.Length - 1)]} <size=80%>{random.Next(1, 200)}<size=100%>\n{(stamp.Country == StampModel.StampCountry.France ? frenchCities[random.Next(0, frenchCities.Length - 1)] : cities[random.Next(0, cities.Length - 1)])}", $"{names[random.Next(0, names.Length - 1)]} {familyNames[random.Next(0, familyNames.Length - 1)]}\n{streets[random.Next(0, streets.Length - 1)]} {random.Next(1, 200)}\n{(stamp.Country == StampModel.StampCountry.France ? frenchCities[random.Next(0, frenchCities.Length - 1)] : cities[random.Next(0, cities.Length - 1)])}", stamp));
    }
}