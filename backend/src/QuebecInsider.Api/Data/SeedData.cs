using Microsoft.EntityFrameworkCore;
using QuebecInsider.Api.Domain;

namespace QuebecInsider.Api.Data;

public static class SeedData
{
    public static async Task InitializeAsync(QuebecInsiderDbContext db)
    {
        if (await db.Articles.AnyAsync())
        {
            return;
        }

        var categories = new[]
        {
            Category("money", "Argent", "argent", "Économies, banques, cartes de crédit, impôts et programmes.", "Money", "money", "Savings, banking, credit cards, taxes and practical programs."),
            Category("deals", "Bons plans", "bons-plans", "Rabais, promotions, Costco, Dollarama, pharmacies et magasinage malin.", "Deals", "deals", "Discounts, promos, Costco, Dollarama, pharmacies and smarter shopping."),
            Category("food", "Bouffe", "bouffe", "Restaurants, épiceries, produits d'ici et découvertes utiles.", "Food", "food", "Restaurants, groceries, Quebec products and useful discoveries."),
            Category("going-out", "Sorties", "sorties", "Activités, festivals, événements gratuits et idées de fin de semaine.", "Things To Do", "things-to-do", "Activities, festivals, free events and weekend ideas."),
            Category("travel", "Voyage", "voyage", "Astuces de transport, destinations québécoises et voyages à prix doux.", "Travel", "travel", "Transit hacks, Quebec destinations and lower-cost travel."),
            Category("tech", "Tech", "tech", "Applications, télécoms, internet et services numériques.", "Tech", "tech", "Apps, telecom, internet and digital services."),
            Category("life", "Vie au Québec", "vie-au-quebec", "Transport, logement, services, français et quotidien québécois.", "Life In Quebec", "life-in-quebec", "Transit, housing, services, French and everyday Quebec life.")
        };

        db.Categories.AddRange(categories);
        await db.SaveChangesAsync();

        db.Articles.AddRange(
            Article(categories[6], 98, "https://images.unsplash.com/photo-1529156069898-49953e39b3ac?auto=format&fit=crop&w=1200&q=80", "Camille Drouin", 8,
                "J'ai appris le français au Québec beaucoup plus vite avec ces séries, balados et classiques",
                "apprendre-francais-quebec-7-trucs",
                "Oubliez le français de manuel scolaire : découvrez comment STAT, Les Bougon, District 31, les balados sans filtre et les Cowboys Fringants m'ont appris le vrai parler québécois.",
                """
                <p class="lead">Quand on débarque au Québec, on réalise vite un secret bien gardé : le français appris à l'école ou dans les livres de grammaire ne prépare absolument pas à la vraie vie d'ici. Entre le débit mitraillette, les expressions savoureuses et les sacres colorés, il y a tout un monde. Pour débloquer mon oreille et parler comme un vrai Québécois, j'ai mis de côté les manuels et j'ai plongé tête première dans la pop culture d'ici. Voici le guide ultime qui a tout changé pour moi.</p>

                <h2>1. Commencer par LA série médicale incontournable : STAT</h2>
                <p>Si vous ne devez écouter qu'une seule série pour démarrer, c'est <strong>STAT</strong>. Pourquoi commencer par celle-là en premier ? Tout simplement parce que c'est le <em>Grey's Anatomy</em> du Québec !</p>
                <p>L'avantage imbattable d'une série hospitalière quotidienne comme STAT, c'est qu'elle brasse absolument toutes les situations sociales imaginables : des urgences médicales intenses, des discussions professionnelles plus posées entre médecins, des drames familiaux déchirants, et les petits potins du quotidien autour de la machine à café. Vous y apprenez un vocabulaire d'une richesse folle, applicable à presque toutes les sphères de la vie courante québécoise.</p>

                <figure>
                  <img src="https://prod-solutionsmedia-cdn.azureedge.net/cache/d/c/5/7/8/c/dc578ca4c0d06b4c27f0f9d141e102049fce7b07.jpg" alt="Suzanne Clément dans la série STAT" />
                  <figcaption>Suzanne Clément incarne le Dr Emmanuelle St-Cyr dans STAT : une véritable clinique linguistique pour capter le rythme québécois.</figcaption>
                </figure>

                <h2>2. Apprivoiser le joual et les vrais jurons avec Les Bougon</h2>
                <p>C'est mon chef au travail qui m'a glissé ce conseil un jour : <em>« Si tu veux vraiment comprendre le Québec d'en bas, écoute Les Bougon! »</em>. Et quel coup de foudre !</p>
                <p>Cette série satirique culte vous plonge directement dans le joual pur et dur, l'humour noir et le parler de rue sans le moindre filtre. C'est là que vous apprenez l'art des jurons et des sacres québécois — non pas pour les crier partout, mais pour enfin saisir toutes leurs nuances émotionnelles. Il y a une fierté indescriptible le jour où vous comprenez une conversation rapide entre deux personnes dans la rue à Montréal, ou le moment magique en fin de soirée où vos amis, un peu éméchés, commencent à sortir les vrais sacres et que vous vibrez enfin au même diapason !</p>

                <figure>
                  <img src="https://images.unsplash.com/photo-1519178173562-b941094191d9?auto=format&fit=crop&w=1200&q=80" alt="Ruelles et escaliers typiques de Montréal" />
                  <figcaption>Des ruelles montréalaises aux soirées entre amis : comprendre le parler de rue transforme votre intégration.</figcaption>
                </figure>

                <h2>3. Le buffet des séries cultes : Du polar à la comédie</h2>
                <p>Le petit écran québécois regorge de pépites pour tous les goûts. Selon vos préférences, piochez dans cette sélection garantie sans temps mort :</p>
                <ul>
                  <li><strong>District 31 :</strong> Le monstre sacré de la télé québécoise pour les amateurs de suspense policier, d'interrogatoires tendus et de grandes intrigues de cœur au poste de police.</li>
                  <li><strong>Les Parent :</strong> Une comédie familiale parfaite pour maîtriser le langage de tous les jours, les chicanes d'ados et les discussions de table à dîner.</li>
                  <li><strong>Watatatow :</strong> Pour une délicieuse touche rétro 90s/2000s. C'est un peu le <em>Friends</em> québécois pour jeunes, idéal pour la nostalgie et les dynamiques de groupe.</li>
                  <li><strong>Les Invincibles :</strong> Imaginez une version masculine et francophone de <em>Sex and the City</em> se déroulant à Montréal : quatre amis trentenaires qui font un pacte pour briser leur routine, avec des dialogues savoureux et hilarants.</li>
                  <li><strong>C'est comme ça que je t'aime :</strong> Un thriller-comédie rétro complètement déjanté, un brin osé, jubilatoire et acclamé par la critique internationale.</li>
                </ul>

                <figure>
                  <img src="https://images.unsplash.com/photo-1509198397868-475647b2a1e5?auto=format&fit=crop&w=1200&q=80" alt="Ambiance nocturne et mystérieuse pour District 31" />
                  <figcaption>District 31 et les polars québécois habituent l'oreille à l'intonation et au débit sous tension.</figcaption>
                </figure>

                <h2>4. Les balados : La parole brute, spontanée et sans filtre</h2>
                <p>Les séries ont un texte écrit, mais les balados (podcasts) vous offrent la parole vivante, avec ses hésitations, ses rires et ses tournures modernes. Enfilez vos écouteurs pendant vos transports ou en faisant la vaisselle :</p>
                <ul>
                  <li><strong>Sexe Oral :</strong> Un des balados les plus écoutés au Québec. C'est drôle, cru, totalement décomplexé et parfait pour apprendre le vocabulaire des relations, du dating et les codes de la jeunesse d'aujourd'hui.</li>
                  <li><strong>En attendant la lettre de Poudlard :</strong> Idéal pour les fans de Harry Potter ! Comme vous connaissez déjà l'histoire, vous n'êtes pas perdu par l'intrigue et votre cerveau peut se concentrer à 100 % sur les expressions et le rythme québécois des animateurs.</li>
                  <li><strong>Tout le monde gagne</strong> et <strong>Ça ou ça :</strong> Des concepts légers de jeux et de débats absurdes entre amis (« Tu préfères ceci ou cela ? »). C'est le meilleur moyen de vous habituer au ping-pong verbal et à la répartie québécoise.</li>
                </ul>

                <figure>
                  <img src="https://images.unsplash.com/photo-1590602847861-f357a9332bbc?auto=format&fit=crop&w=1200&q=80" alt="Studio de baladodiffusion" />
                  <figcaption>Écouter des balados locaux permet de capter les tics de langage et le débit réel des conversations.</figcaption>
                </figure>

                <h2>5. La musique et le country québécois : Là où vivent les vrais mots</h2>
                <p>On n'y pense pas assez, mais la musique est une arme secrète. Mon grand conseil : plongez dans le country québécois et les grands classiques de la chanson d'ici.</p>
                <p>Pourquoi ? Parce que ces chansons racontent la vie simple, la terre, les dépanneurs, les froids de canard et les histoires d'amour du coin. Des groupes comme <strong>Les Cowboys Fringants</strong> (ou des artistes comme Paul Piché et Kaïn) utilisent des mots populaires, imagés et poétiques que vous n'entendrez jamais dans un cours de français traditionnel. Écouter les Cowboys autour d'un feu de camp avec des Québécois qui chantent à tue-tête, c'est l'examen final réussi de votre intégration !</p>

                <figure>
                  <img src="https://images.unsplash.com/photo-1511671782779-c97d3d27a1d4?auto=format&fit=crop&w=1200&q=80" alt="Guitare et chanson québécoise" />
                  <figcaption>Les classiques des Cowboys Fringants ouvrent les portes d'un vocabulaire imagé et chaleureux.</figcaption>
                </figure>
                """,
                "français, intégration, vie au Québec, séries télé, balados, musique québécoise, bibliothèques",
                "Empruntez gratuitement les coffrets de séries québécoises (STAT, District 31, Les Bougon) ou des livres audio via votre bibliothèque municipale avant de payer des abonnements !",
                "I Learned Quebec French Way Faster Binge-Watching TV Shows, Podcasts, and Classics",
                "learn-french-quebec-7-strategies",
                "STAT, Les Bougon, District 31, uncensored podcasts, and Les Cowboys Fringants: how Quebec pop culture taught me the real language faster than any textbook.",
                """
                <p class="lead">When you first move to Quebec, you quickly discover a well-kept secret: classroom French simply does not prepare you for everyday life here. Between the rapid-fire cadence, colourful slang, and expressive expletives, it is a whole different world. To train my ear and speak like a local, I put textbooks aside and dove headfirst into Quebec pop culture. Here is the ultimate playbook that changed everything for me.</p>

                <h2>1. Start With THE Must-Watch Medical Drama: STAT</h2>
                <p>If you only watch one show to begin with, make it <strong>STAT</strong>. Why start here? Because it is Quebec's very own <em>Grey's Anatomy</em>!</p>
                <p>The unbeatable perk of a daily hospital drama is the sheer variety of social dynamics: high-stakes medical emergencies, polished professional consultations among doctors, heartbreaking family conflicts, and casual breakroom banter around the coffee machine. You will absorb a diverse vocabulary applicable to almost every corner of daily life.</p>

                <figure>
                  <img src="https://prod-solutionsmedia-cdn.azureedge.net/cache/d/c/5/7/8/c/dc578ca4c0d06b4c27f0f9d141e102049fce7b07.jpg" alt="Suzanne Clément in the STAT TV series" />
                  <figcaption>Suzanne Clément stars as Dr. Emmanuelle St-Cyr in STAT: the ultimate linguistic immersion for natural conversational speed.</figcaption>
                </figure>

                <h2>2. Tame Street Slang and Real Expletives With Les Bougon</h2>
                <p>A boss at work gave me this advice: <em>"If you truly want to understand Quebec from the ground up, you have to watch Les Bougon!"</em> And what a revelation it was!</p>
                <p>This cult satire drops you straight into unfiltered street French and dark comedy. It is the masterclass in Quebec expletives ("sacres")—not so you can swear everywhere, but so you can finally grasp their emotional weight and humor. Nothing beats the pride of understanding fast-paced street banter in Montreal or sharing laughs with friends at a party when the real expressions start flying!</p>

                <figure>
                  <img src="https://images.unsplash.com/photo-1519178173562-b941094191d9?auto=format&fit=crop&w=1200&q=80" alt="Authentic Montreal street scene" />
                  <figcaption>From neighborhood staircases to evening drinks with friends: unlocking colloquial French changes everything.</figcaption>
                </figure>

                <h2>3. The Iconic Binge-List: Crime, Drama & Comedy</h2>
                <p>Quebec television is filled with gems for every taste:</p>
                <ul>
                  <li><strong>District 31:</strong> The landmark daily police procedural for intense investigations, gripping suspense, and precinct drama.</li>
                  <li><strong>Les Parent:</strong> A heartwarming family comedy capturing authentic domestic banter, teen slang, and dinner table chaos.</li>
                  <li><strong>Watatatow:</strong> A nostalgic 90s/2000s staple—often seen as the Quebec teen version of <em>Friends</em>.</li>
                  <li><strong>Les Invincibles:</strong> Think of an all-male francophone <em>Sex and the City</em> set in Montreal: four 30-something buddies navigating adulthood and relationships with hilarious honesty.</li>
                  <li><strong>C'est comme ça que je t'aime:</strong> An award-winning, darkly funny retro-thriller set in the 1970s that is wild, spicy, and endlessly entertaining.</li>
                </ul>

                <figure>
                  <img src="https://images.unsplash.com/photo-1509198397868-475647b2a1e5?auto=format&fit=crop&w=1200&q=80" alt="District 31 noir crime vibe" />
                  <figcaption>Police dramas like District 31 train your ear to fast interrogations and realistic cadence.</figcaption>
                </figure>

                <h2>4. Podcasts: Raw, Unfiltered Conversations</h2>
                <p>While TV shows follow scripts, podcasts give you living, breathing speech with genuine pauses, laughter, and modern expressions:</p>
                <ul>
                  <li><strong>Sexe Oral:</strong> One of Quebec's most popular podcasts. Honest, funny, and direct—a goldmine for modern dating and relationship vocabulary.</li>
                  <li><strong>En attendant la lettre de Poudlard:</strong> Perfect for Harry Potter fans! Since you already know the story, your brain can focus entirely on picking up local phrasing and comedic banter.</li>
                  <li><strong>Tout le monde gagne</strong> and <strong>Ça ou ça:</strong> Lighthearted debate and game shows ("Would you rather...") that sharpen your comprehension of quick-witted banter.</li>
                </ul>

                <figure>
                  <img src="https://images.unsplash.com/photo-1590602847861-f357a9332bbc?auto=format&fit=crop&w=1200&q=80" alt="Podcast recording studio" />
                  <figcaption>Local podcasts are the fastest way to get comfortable with natural speech rhythm.</figcaption>
                </figure>

                <h2>5. Music & Quebec Country: Where Words Come to Life</h2>
                <p>Music is an underrated learning superpower. Dive into classic Quebec songs and country folk.</p>
                <p>Why? Because these songs tell stories of ordinary life: corner cornerstores ("dépanneurs"), bone-chilling winters, and heartfelt ballads. Bands like <strong>Les Cowboys Fringants</strong> (along with legends like Paul Piché and Kaïn) use vibrant, everyday language you will never encounter in a textbook. Singing along to Les Cowboys around a campfire with local friends is the ultimate badge of integration!</p>

                <figure>
                  <img src="https://images.unsplash.com/photo-1511671782779-c97d3d27a1d4?auto=format&fit=crop&w=1200&q=80" alt="Acoustic guitar and campfire vibes" />
                  <figcaption>Les Cowboys Fringants songs invite you into the poetic and warm heart of Quebec culture.</figcaption>
                </figure>
                """,
                "French, integration, life in Quebec, TV shows, podcasts, Quebec music, libraries",
                "Borrow Quebec TV series box sets (STAT, District 31, Les Bougon) or audiobooks for free from your local municipal library before paying for streaming subscriptions!"),

            Article(categories[0], 94, "https://images.unsplash.com/photo-1554224154-26032ffc0d07?auto=format&fit=crop&w=1200&q=80", "Nora Beaulieu", 6,
                "Nouveau au Québec? Les avantages bancaires que tu risques de laisser passer",
                "nouveau-quebec-avantages-bancaires",
                "Les éléments à comparer avant d'ouvrir un premier compte bancaire au Québec, sans se laisser distraire uniquement par le cadeau de bienvenue.",
                """
                Le cadeau de bienvenue n'est qu'un élément
                Une prime peut attirer l'attention, mais elle ne dit pas si le compte est avantageux après quelques mois. Regarde les conditions, les frais et les services réellement nécessaires.

                Comparer les frais mensuels
                Les frais de compte varient selon le forfait. Vérifie ce qui est inclus avant de choisir, surtout si tu fais beaucoup de transactions.

                Vérifier les conditions d'exonération
                Certains comptes peuvent réduire ou éliminer les frais si un solde minimal ou d'autres conditions sont respectés. Ces conditions peuvent changer; vérifie toujours la page officielle de l'institution.

                Examiner les limites de transactions
                Les retraits, virements, paiements Interac ou opérations au comptoir peuvent être limités selon le forfait. Une limite trop basse peut coûter cher.

                Comparer les cartes de crédit pour nouveaux arrivants
                Certaines institutions ont des produits pour bâtir un historique de crédit. Compare les frais, taux, assurances et conditions plutôt que seulement la limite annoncée.

                Vérifier les virements internationaux
                Si tu envoies ou reçois de l'argent à l'étranger, compare les frais, taux de change et délais. Vérifié le 24 août 2026: les conditions doivent être confirmées sur les sites officiels des institutions.

                Comprendre les conditions et dates limites
                Une offre peut demander un dépôt direct, des paiements automatisés ou une ouverture avant une date précise. Note les exigences avant d'ouvrir le compte.

                Faire un calcul sur douze mois
                Additionne les frais, soustrais la valeur sûre de l'offre et ignore ce qui dépend de conditions que tu ne rempliras peut-être pas.
                """,
                "banques, nouveaux arrivants, finances personnelles, Québec",
                "Calcule la valeur nette de l'offre après douze mois : récompense moins frais mensuels, coûts de carte et services payants obligatoires.",
                "New to Quebec? Banking Benefits You Might Be Missing",
                "new-quebec-banking-benefits",
                "What to compare before opening your first Quebec bank account instead of focusing only on the welcome offer.",
                """
                The welcome offer is only one piece
                A bonus can be attractive, but it does not tell you whether the account is useful after the first few months. Look at conditions, fees and services you will actually use.

                Compare monthly fees
                Account fees vary by package. Check what is included before choosing, especially if you expect frequent transactions.

                Check fee-waiver conditions
                Some accounts reduce or waive fees when a minimum balance or other condition is met. These rules can change; always verify the institution's official page.

                Review transaction limits
                Withdrawals, transfers, Interac payments or in-branch transactions may be limited depending on the package. A limit that is too low can become expensive.

                Compare credit cards for newcomers
                Some institutions offer products meant to help build credit history. Compare fees, rates, insurance and conditions instead of focusing only on the advertised limit.

                Check international transfers
                If you send or receive money abroad, compare fees, exchange rates and timing. Verified August 24, 2026: conditions should be confirmed on official institution websites.

                Understand conditions and deadlines
                An offer may require direct deposit, automated payments or opening before a specific date. Write down the requirements before opening the account.

                Calculate the 12-month value
                Add the fees, subtract the reliable value of the offer and ignore benefits tied to conditions you may not meet.
                """,
                "banking, newcomers, personal finance, Quebec",
                "Calculate the offer's net value over 12 months: reward minus monthly fees, card costs and any required paid services."),

            Article(categories[2], 91, "https://images.unsplash.com/photo-1542838132-92c53300491e?auto=format&fit=crop&w=1200&q=80", "Mathieu Gagnon", 6,
                "J'ai arrêté de tout acheter au même endroit : ce qui vaut vraiment Costco",
                "ce-qui-vaut-vraiment-costco-quebec",
                "Une méthode simple pour décider quels produits acheter en gros et lesquels comparer chez Maxi, Super C ou Walmart.",
                """
                Comparer le prix par unité
                Le bon réflexe est de comparer le prix par 100 g, par litre ou par unité. Le gros format gagne parfois, mais pas toujours.

                Tenir compte du gaspillage
                Un produit moins cher devient cher si une partie finit à la poubelle. Les formats familiaux conviennent mieux aux produits que tu consommes vraiment.

                Séparer produits de base et promotions
                Un produit de base acheté souvent mérite une comparaison régulière. Une promotion ponctuelle demande seulement de vérifier si tu en as réellement besoin.

                Vérifier l'espace de rangement
                L'économie disparaît si les gros formats encombrent ton logement ou t'obligent à acheter du rangement supplémentaire.

                Calculer la valeur réelle de l'abonnement
                Ne calcule pas seulement les économies potentielles. Compare aussi le coût de l'abonnement, la distance, le temps et tes habitudes.

                Comparer avant chaque achat important
                Les prix changent. Vérifié le 24 août 2026: les prix d'épicerie doivent être comparés au moment de l'achat dans les circulaires ou sites officiels des détaillants.

                Produits à longue conservation contre produits périssables
                Les articles non périssables sont plus faciles à acheter en gros. Pour le frais, sois plus strict avec la quantité et la date de consommation.
                """,
                "Costco, épicerie, budget, alimentation, Québec",
                "Le plus gros emballage n'est pas automatiquement le moins cher : compare toujours le prix par 100 g, par litre ou par unité.",
                "I Stopped Buying Everything in One Place: What Is Actually Worth It at Costco",
                "what-is-worth-buying-costco-quebec",
                "A simple method for deciding what to buy in bulk and what to compare at Maxi, Super C or Walmart.",
                """
                Compare the unit price
                The useful habit is comparing the price per 100 g, litre or unit. Bulk packaging sometimes wins, but not automatically.

                Account for waste
                A cheaper product becomes expensive if part of it ends up in the trash. Family sizes work best for items you truly use.

                Separate staples from promotions
                A staple you buy often deserves regular comparison. A temporary promotion only matters if you actually need the item.

                Check your storage space
                Savings fade if bulk purchases crowd your apartment or force you to buy extra storage.

                Calculate the real membership value
                Do not count only possible savings. Include membership cost, distance, time and your shopping habits.

                Compare before each major purchase
                Prices change. Verified August 24, 2026: grocery prices should be compared at purchase time through flyers or official retailer websites.

                Shelf-stable goods versus perishables
                Non-perishable items are easier to buy in bulk. For fresh food, be stricter about quantity and timing.
                """,
                "Costco, groceries, budget, food, Quebec",
                "The largest package is not automatically the cheapest: always compare the price per 100 g, litre or unit."),

            Article(categories[0], 87, "https://images.unsplash.com/photo-1523240795612-9a054b0db644?auto=format&fit=crop&w=1200&q=80", "Sofia Tremblay", 7,
                "Étudiants à Montréal : l'argent que beaucoup oublient de demander",
                "aide-financiere-etudiants-montreal",
                "Une liste de catégories d'aide à vérifier auprès de son établissement, des gouvernements et des services locaux.",
                """
                Bourses propres à l'établissement
                Les universités, cégeps et écoles ont souvent leurs propres bourses. Elles peuvent dépendre du programme, du dossier, du besoin financier ou d'une situation particulière.

                Aide financière aux études du Québec
                L'AFE est la porte d'entrée officielle pour les prêts et bourses du Québec. Vérifié le 24 août 2026: les critères doivent être confirmés sur Québec.ca.

                Bourses liées au programme
                Certains départements publient des bourses discrètes ou peu publicisées. Vérifie les pages de ton programme et demande au secrétariat.

                Fonds d'urgence
                Plusieurs établissements ont des fonds pour une difficulté temporaire. Ce n'est pas universel; il faut confirmer l'admissibilité auprès de son école.

                Soutien pour le logement ou l'alimentation
                Associations étudiantes, banques alimentaires, services municipaux et organismes locaux peuvent offrir du soutien. Les conditions varient beaucoup.

                Crédits d'impôt et déclarations de revenus
                Même avec peu de revenus, une déclaration peut être nécessaire pour certains crédits ou prestations. Vérifie auprès de Revenu Québec et de l'ARC.

                Réductions étudiantes
                Transport, logiciels, musées, activités et services peuvent proposer des tarifs étudiants. Demande toujours si une preuve d'inscription suffit.

                Calendrier personnel des échéances
                La meilleure aide est souvent celle demandée à temps. Mets les dates limites dans un calendrier et garde les documents de base prêts.
                """,
                "étudiants, Montréal, bourses, AFE, finances",
                "Même avec peu ou aucun revenu, produire une déclaration de revenus peut être nécessaire pour recevoir certains crédits et prestations. Vérifie toujours ton admissibilité auprès de Revenu Québec et de l'ARC.",
                "Montreal Students: Funding Many People Forget to Apply For",
                "financial-support-montreal-students",
                "Categories of assistance worth checking through your institution, governments and local services.",
                """
                Institution-specific scholarships
                Universities, CEGEPs and schools often have their own awards. They may depend on program, grades, financial need or a specific situation.

                Quebec student financial assistance
                AFE is the official entry point for Quebec loans and bursaries. Verified August 24, 2026: eligibility must be confirmed on Québec.ca.

                Program-related awards
                Some departments publish smaller or less visible scholarships. Check your program pages and ask the department office.

                Emergency funds
                Many institutions have funds for temporary hardship. This is not universal; confirm eligibility with your school.

                Housing or food support
                Student associations, food banks, municipal services and local organizations may offer support. Conditions vary widely.

                Tax credits and income tax returns
                Even with little income, filing a return may be needed for certain credits or benefits. Check with Revenu Québec and the CRA.

                Student discounts
                Transit, software, museums, activities and services may offer student rates. Always ask whether proof of enrolment is enough.

                A personal deadline calendar
                The best support is often the one requested on time. Put deadlines in a calendar and keep basic documents ready.
                """,
                "students, Montreal, scholarships, financial aid, taxes",
                "Even with little or no income, filing a tax return may be necessary to receive certain credits and benefits. Always verify eligibility with Revenu Québec and the CRA."),

            Article(categories[5], 84, "https://images.unsplash.com/photo-1516321497487-e288fb19713f?auto=format&fit=crop&w=1200&q=80", "Marc-André Roy", 5,
                "Ton forfait mobile te coûte-t-il trop cher? La comparaison québécoise à faire chaque année",
                "comparer-forfait-mobile-quebec",
                "Une méthode annuelle pour comparer son forfait, sa couverture et ses besoins réels sans courir après chaque promotion.",
                """
                Vérifier sa consommation réelle de données
                Consulte quelques mois de factures et regarde ta consommation moyenne. Un forfait plus gros que nécessaire n'est pas automatiquement plus pratique.

                Comparer le prix avant taxes
                Les taxes et frais peuvent rendre les comparaisons floues. Compare d'abord le prix mensuel avant taxes, puis ajoute les frais obligatoires.

                Examiner la couverture dans ses lieux habituels
                La meilleure offre sur papier ne vaut pas grand-chose si la couverture est faible chez toi, au travail ou sur tes trajets fréquents.

                Vérifier les frais d'activation et de changement
                Avant de changer, cherche les frais d'activation, de carte SIM, de transfert ou de résiliation qui pourraient s'appliquer.

                Distinguer financement du téléphone et prix du forfait
                Un paiement mensuel peut mélanger appareil et service. Sépare les deux pour comprendre le vrai coût du forfait.

                Surveiller les promotions sans changer inutilement
                Les promotions reviennent souvent. Note-les, mais change seulement si l'économie est claire et si la couverture répond à tes besoins.

                Refaire la comparaison une fois par année
                Vérifié le 24 août 2026: les offres mobiles changent souvent; les conditions doivent être confirmées sur les sites officiels des fournisseurs.
                """,
                "forfait mobile, télécoms, Québec, budget, données",
                "Consulte quelques mois de factures : ton utilisation réelle est plus utile que le nombre de gigaoctets affiché dans une publicité.",
                "Are You Overpaying for Your Phone Plan? The Quebec Comparison to Do Every Year",
                "compare-mobile-plans-quebec",
                "A yearly method for comparing your plan, coverage and actual needs without chasing every promotion.",
                """
                Check your real data usage
                Review several months of bills and look at your average usage. A bigger plan is not automatically more useful.

                Compare the pre-tax price
                Taxes and fees can blur comparisons. Start with the monthly price before tax, then add required charges.

                Review coverage where you actually go
                The best offer on paper is not worth much if coverage is weak at home, at work or on your usual routes.

                Check activation and switching fees
                Before changing providers, look for activation, SIM, transfer or cancellation fees that may apply.

                Separate phone financing from plan price
                A monthly payment can mix device and service. Separate both to understand the true plan cost.

                Watch promotions without switching constantly
                Promotions come back often. Track them, but switch only when the savings are clear and coverage fits your needs.

                Repeat the comparison once a year
                Verified August 24, 2026: mobile offers change often; conditions should be confirmed on official provider websites.
                """,
                "mobile plans, telecom, Quebec, budget, data",
                "Review several months of bills: your real usage matters more than the number of gigabytes advertised."));

        await db.SaveChangesAsync();
    }

    private static Category Category(string key, string frName, string frSlug, string frDescription, string enName, string enSlug, string enDescription) =>
        new()
        {
            Key = key,
            Translations =
            [
                new() { Culture = "fr-CA", Name = frName, Slug = frSlug, Description = frDescription },
                new() { Culture = "en-CA", Name = enName, Slug = enSlug, Description = enDescription }
            ]
        };

    private static Article Article(Category category, int popularity, string imageUrl, string author, int readingMinutes,
        string frTitle, string frSlug, string frDescription, string frBody, string frKeywords, string frInsiderTip,
        string enTitle, string enSlug, string enDescription, string enBody, string enKeywords, string enInsiderTip) =>
        new()
        {
            Category = category,
            PopularityScore = popularity,
            ImageUrl = imageUrl,
            AuthorName = author,
            ReadingMinutes = readingMinutes,
            PublishedAt = DateTimeOffset.UtcNow.AddDays(-(100 - popularity)),
            Translations =
            [
                new()
                {
                    Culture = "fr-CA",
                    Title = frTitle,
                    Slug = frSlug,
                    Description = frDescription,
                    Body = frBody,
                    Keywords = frKeywords,
                    InsiderTip = frInsiderTip,
                    SeoTitle = $"{frTitle} | QuébecInsider",
                    SeoDescription = frDescription
                },
                new()
                {
                    Culture = "en-CA",
                    Title = enTitle,
                    Slug = enSlug,
                    Description = enDescription,
                    Body = enBody,
                    Keywords = enKeywords,
                    InsiderTip = enInsiderTip,
                    SeoTitle = $"{enTitle} | QuébecInsider",
                    SeoDescription = enDescription
                }
            ]
        };
}
