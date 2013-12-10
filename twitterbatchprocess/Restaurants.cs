using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace twitterbatchprocess
{
    class Restaurants
    {
 
        public string PlaceName { get; set; }

    }
    class RestaurantList
    {
        private List<String> _PlaceList;
        public List<String> PlaceList
        {
            get
            {
                if (_PlaceList == null)
                {
                    _PlaceList = new List<String>();

                    _PlaceList.Add("	Aberdeen Angus Steak Houses	 ".Trim());
                    _PlaceList.Add("	All Bar One	 ".Trim());
                    _PlaceList.Add("	AMT Coffee	 ".Trim());
                    _PlaceList.Add("	Ask	 ".Trim());
                    _PlaceList.Add("	Bagel Nash	 ".Trim());
                    _PlaceList.Add("	Beefeater	 ".Trim());
                    _PlaceList.Add("	Bella Italia	 ".Trim());
                    _PlaceList.Add("	Brewers Fayre	 ".Trim());
                    _PlaceList.Add("	Café Rouge	 ".Trim());
                    _PlaceList.Add("	Caffè Nero	 ".Trim());
                    _PlaceList.Add("	Chicken Cottage	 ".Trim());
                    _PlaceList.Add("	Chiquito	 ".Trim());
                    _PlaceList.Add("	Coffee Republic	 ".Trim());
                    _PlaceList.Add("	Costa Coffee	 ".Trim());
                    _PlaceList.Add("	EasyPizza	 ".Trim());
                    _PlaceList.Add("	EAT.	 ".Trim());
                    _PlaceList.Add("	Fine Burger Company	 ".Trim());
                    _PlaceList.Add("	Frankie & Benny's	 ".Trim());
                    _PlaceList.Add("	Giraffe Restaurants	 ".Trim());
                    _PlaceList.Add("	Gourmet Burger Kitchen	 ".Trim());
                    _PlaceList.Add("	Greggs	 ".Trim());
                    _PlaceList.Add("	Harry Ramsden's	 ".Trim());
                    _PlaceList.Add("	Harvester	 ".Trim());
                    _PlaceList.Add("	Hungry Horse	 ".Trim());
                    _PlaceList.Add("	J D Wetherspoon	 ".Trim());
                    _PlaceList.Add("	Little Chef	 ".Trim());
                    _PlaceList.Add("	Loch Fyne	 ".Trim());
                    _PlaceList.Add("	Millie's Cookies	 ".Trim());
                    _PlaceList.Add("	OK Diner	 ".Trim());
                    _PlaceList.Add("	Pizza Hut	 ".Trim());
                    _PlaceList.Add("	Pret A Manger	 ".Trim());
                    _PlaceList.Add("	Prezzo	 ".Trim());
                    _PlaceList.Add("	Puccino's	 ".Trim());
                    _PlaceList.Add("	Punch Taverns	 ".Trim());
                    _PlaceList.Add("	S & M Cafe	 ".Trim());
                    _PlaceList.Add("	Scream Pubs	 ".Trim());
                    _PlaceList.Add("	Shakeaway	 ".Trim());
                    _PlaceList.Add("	Slug and Lettuce	 ".Trim());
                    _PlaceList.Add("	Spudulike	 ".Trim());
                    _PlaceList.Add("	Strada	 ".Trim());
                    _PlaceList.Add("	Subway	 ".Trim());
                    _PlaceList.Add("	Table Table	 ".Trim());
                    _PlaceList.Add("	Taybarns	 ".Trim());
                    _PlaceList.Add("	Upper Crust	 ".Trim());
                    _PlaceList.Add("	Wagamama	 ".Trim());
                    _PlaceList.Add("	Walkabout	 ".Trim());
                    _PlaceList.Add("	The West Cornwall Pasty Company	 ".Trim());
                    _PlaceList.Add("	Wimpy	 ".Trim());
                    _PlaceList.Add("	Yates's	 ".Trim());
                    _PlaceList.Add("	YO! Sushi	 ".Trim());
                    _PlaceList.Add("	Zizzi	 ".Trim());
                    _PlaceList.Add("	Applebee's	 ".Trim());
                    _PlaceList.Add("	Arby's	 ".Trim());
                    _PlaceList.Add("	Bonchon Chicken	 ".Trim());
                    _PlaceList.Add("	Buffalo Wild Wings	 ".Trim());
                    _PlaceList.Add("	Burger King	 ".Trim());
                    _PlaceList.Add("	Carl's Jr.	 ".Trim());
                    _PlaceList.Add("	Charley's Grilled Subs	 ".Trim());
                    _PlaceList.Add("	Chili's	 ".Trim());
                    _PlaceList.Add("	Chick-fil-A	 ".Trim());
                    _PlaceList.Add("	Chipotle Mexican Grill	 ".Trim());
                    _PlaceList.Add("	Corner Bakery Cafe	 ".Trim());
                    _PlaceList.Add("	coffeeheaven	 ".Trim());
                    _PlaceList.Add("	Costa Coffee	 ".Trim());
                    _PlaceList.Add("	Crepes & Waffles	 ".Trim());
                    _PlaceList.Add("	Délifrance	 ".Trim());
                    _PlaceList.Add("	Denny's	 ".Trim());
                    _PlaceList.Add("	Dome	 ".Trim());
                    _PlaceList.Add("	Din Tai Fung	 ".Trim());
                    _PlaceList.Add("	Domino's Pizza	 ".Trim());
                    _PlaceList.Add("	Gloria Jean's Coffees	 ".Trim());
                    _PlaceList.Add("	Hamburguesas El Corral	 ".Trim());
                    _PlaceList.Add("	Hard Rock Cafe	 ".Trim());
                    _PlaceList.Add("	Hardee's	 ".Trim());
                    _PlaceList.Add("	Hooters	 ".Trim());
                    _PlaceList.Add("	In-N-Out Burger	 ".Trim());
                    _PlaceList.Add("	Jack Astor's Bar & Grill	 ".Trim());
                    _PlaceList.Add("	Jack in the Box	 ".Trim());
                    _PlaceList.Add("	Jollibee	 ".Trim());
                    _PlaceList.Add("	Juan Valdez Cafe	 ".Trim());
                    _PlaceList.Add("	The Keg	 ".Trim());
                    _PlaceList.Add("	Kenny Rogers Roasters	 ".Trim());
                    _PlaceList.Add("	KFC	 ".Trim());
                    _PlaceList.Add("	Krispy Kreme	 ".Trim());
                    _PlaceList.Add("	Little Caesars	 ".Trim());
                    _PlaceList.Add("	Loving Hut	 ".Trim());
                    _PlaceList.Add("	Marrybrown	 ".Trim());
                    _PlaceList.Add("	McDonald's	 ".Trim());
                    _PlaceList.Add("	MOS Burger	 ".Trim());
                    _PlaceList.Add("	Nando's	 ".Trim());
                    _PlaceList.Add("	Old Country Buffet	 ".Trim());
                    _PlaceList.Add("	Olive Garden	 ".Trim());
                    _PlaceList.Add("	Outback Steakhouse	 ".Trim());
                    _PlaceList.Add("	Panera Bread	 ".Trim());
                    _PlaceList.Add("	Papa John's Pizza	 ".Trim());
                    _PlaceList.Add("	Paris Baguette	 ".Trim());
                    _PlaceList.Add("	Perkins Restaurant & Bakery	 ".Trim());
                    _PlaceList.Add("	Pizza Express	 ".Trim());
                    _PlaceList.Add("	Pizza Hut	 ".Trim());
                    _PlaceList.Add("	Planet Hollywood	 ".Trim());
                    _PlaceList.Add("	Pollo Campero	 ".Trim());
                    _PlaceList.Add("	Popeyes Chicken & Biscuits	 ".Trim());
                    _PlaceList.Add("	Quiznos	 ".Trim());
                    _PlaceList.Add("	Rainforest Cafe	 ".Trim());
                    _PlaceList.Add("	Red Lobster	 ".Trim());
                    _PlaceList.Add("	Red Robin	 ".Trim());
                    _PlaceList.Add("	Ruby Tuesday	 ".Trim());
                    _PlaceList.Add("	Sarku Japan	 ".Trim());
                    _PlaceList.Add("	Secret Recipe	 ".Trim());
                    _PlaceList.Add("	Shakey's Pizza	 ".Trim());
                    _PlaceList.Add("	Sizzler	 ".Trim());
                    _PlaceList.Add("	Sonic Drive-In	 ".Trim());
                    _PlaceList.Add("	Starbucks	 ".Trim());
                    _PlaceList.Add("	Steak 'n Shake	 ".Trim());
                    _PlaceList.Add("	Subway	 ".Trim());
                    _PlaceList.Add("	Sweet Frog	 ".Trim());
                    _PlaceList.Add("	Swensen's	 ".Trim());
                    _PlaceList.Add("	Taco Bell	 ".Trim());
                    _PlaceList.Add("	Telepizza	 ".Trim());
                    _PlaceList.Add("	Tim Horton's	 ".Trim());
                    _PlaceList.Add("	T.G.I. Friday's	 ".Trim());
                    _PlaceList.Add("	Tony Roma's	 ".Trim());
                    _PlaceList.Add("	Wendy's	 ".Trim());
                    _PlaceList.Add("	Yogen Früz	 ".Trim());
                    _PlaceList.Add("	Yoshinoya	 ".Trim());






                }
                return _PlaceList;
            }
        }
    }
}
