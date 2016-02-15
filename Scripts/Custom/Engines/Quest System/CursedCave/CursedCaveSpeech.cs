using System;
using System.Text;
using System.Collections;
using Server;

namespace Server.Misc
{
	public class CursedCaveSpeech
	{
		private static InhumanSpeech m_CursedSpeech;

		public static InhumanSpeech Cursed
		{
			get
			{
				if ( m_CursedSpeech == null )
				{
					m_CursedSpeech = new InhumanSpeech();

					m_CursedSpeech.Hue = 472;
					m_CursedSpeech.Sound = 432;

					m_CursedSpeech.Flags = IHSFlags.All;

					m_CursedSpeech.Keywords = new string[]
						{
							"meat", "gold", "kill", "killing", "slay",
							"sword", "axe", "spell", "magic", "spells",
							"swords", "axes", "mace", "maces", "monster",
							"monsters", "food", "run", "escape", "away",
							"help", "dead", "die", "dying", "lose",
							"losing", "life", "lives", "death", "ghost",
							"ghosts", "british", "blackthorn", "guild",
							"guilds", "dragon", "dragons", "game", "games",
							"ultima", "silly", "stupid", "dumb", "idiot",
							"idiots", "cheesy", "cheezy", "crazy", "dork",
							"jerk", "fool", "foolish", "ugly", "insult", "scum"
						};

					m_CursedSpeech.Responses = new string[]
						{
							"meat", "kill", "pound", "crush", "yum yum",
							"crunch", "destroy", "murder", "eat", "munch",
							"massacre", "food", "monster", "evil", "run",
							"die", "lose", "dumb", "idiot", "fool", "crazy",
							"dinner", "lunch", "breakfast", "fight", "battle",
							"doomed", "rip apart", "tear apart", "smash",
							"edible?", "shred", "disembowel", "ugly", "smelly",
							"stupid", "hideous", "smell", "tasty", "invader",
							"attack", "raid", "plunder", "pillage", "treasure",
							"loser", "lose", "scum"
						};

					m_CursedSpeech.Syllables = new string[]
						{
							"skrit",

							"ch", "ch",
							"it", "ti", "it", "ti",

							"ak", "ek", "ik", "ok", "uk", "yk",
							"ka", "ke", "ki", "ko", "ku", "ky",
							"at", "et", "it", "ot", "ut", "yt",

							"cha", "che", "chi", "cho", "chu", "chy",
							"ach", "ech", "ich", "och", "uch", "ych",
							"att", "ett", "itt", "ott", "utt", "ytt",
							"tat", "tet", "tit", "tot", "tut", "tyt",
							"tta", "tte", "tti", "tto", "ttu", "tty",
							"tak", "tek", "tik", "tok", "tuk", "tyk",
							"ack", "eck", "ick", "ock", "uck", "yck",
							"cka", "cke", "cki", "cko", "cku", "cky",
							"rak", "rek", "rik", "rok", "ruk", "ryk",

							"tcha", "tche", "tchi", "tcho", "tchu", "tchy",
							"rach", "rech", "rich", "roch", "ruch", "rych",
							"rrap", "rrep", "rrip", "rrop", "rrup", "rryp",
							"ccka", "ccke", "ccki", "ccko", "ccku", "ccky"
						};
				}

				return m_CursedSpeech;
			}
		}
	}
}