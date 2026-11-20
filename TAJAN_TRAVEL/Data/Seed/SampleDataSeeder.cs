//using TAJAN_TRAVEL.Models.Entities;

//namespace TAJAN_TRAVEL.Data.Seed
//{
//    /// <summary>
//    /// بيانات تجريبية لكل الجداول. آمن تشغيله أكتر من مرة:
//    /// الـ Destinations والـ Tours بتتضاف بالـ Slug لو مش موجودة،
//    /// وباقي الجداول بتتملي بس لو فاضية.
//    /// </summary>
//    public static class SampleDataSeeder
//    {
//        public static void Seed(ApplicationDbContext db)
//        {
//            SeedActivityTypes(db);
//            SeedDestinations(db);
//            SeedTours(db);
//            SeedBlog(db);
//            EventSeeder.Seed(db);
//            SeedTestimonials(db);
//            SeedFaqs(db);
//            SeedInquiries(db);
//            SeedContactMessages(db);
//            StorySeeder.Seed(db); // keep last: adds stories/reviews on top of the older sample data
//        }

//        // ---------------------------------------------------------------- Activity types
//        private static void SeedActivityTypes(ApplicationDbContext db)
//        {
//            if (db.ActivityTypes.Any()) return;

//            db.ActivityTypes.AddRange(
//                new ActivityType { NameEn = "Culture & History", NameFr = "Culture et Histoire", IconClass = "landmark" },
//                new ActivityType { NameEn = "Adventure", NameFr = "Aventure", IconClass = "compass" },
//                new ActivityType { NameEn = "Relaxation", NameFr = "Detente", IconClass = "sun" },
//                new ActivityType { NameEn = "Desert & Oasis", NameFr = "Desert et Oasis", IconClass = "mountain" },
//                new ActivityType { NameEn = "Nile Sailing", NameFr = "Navigation sur le Nil", IconClass = "sailboat" }
//            );
//            db.SaveChanges();
//        }

//        // ---------------------------------------------------------------- Destinations
//        private static void SeedDestinations(ApplicationDbContext db)
//        {
//            var existing = db.Destinations.Select(d => d.Slug).ToHashSet();
//            var maxOrder = db.Destinations.Any() ? db.Destinations.Max(d => d.DisplayOrder) : 0;

//            var toAdd = new List<Destination>();

//            if (!existing.Contains("hurghada"))
//                toAdd.Add(new Destination
//                {
//                    NameEn = "Hurghada",
//                    NameFr = "Hurghada",
//                    Slug = "hurghada",
//                    DescriptionEn = "Red Sea beaches, snorkelling, diving and desert jeep safaris.",
//                    DescriptionFr = "Plages de la mer Rouge, snorkeling, plongee et safaris en jeep.",
//                    Category = "Coastal Escapes",
//                    DisplayOrder = ++maxOrder
//                });

//            if (!existing.Contains("bahariya"))
//                toAdd.Add(new Destination
//                {
//                    NameEn = "Bahariya Oasis",
//                    NameFr = "Oasis de Bahariya",
//                    Slug = "bahariya",
//                    DescriptionEn = "Gateway to the Black and White Deserts, with hot springs and starry nights.",
//                    DescriptionFr = "Porte des deserts Noir et Blanc, avec sources chaudes et nuits etoilees.",
//                    Category = "Desert & Oasis",
//                    DisplayOrder = ++maxOrder
//                });

//            if (toAdd.Count > 0)
//            {
//                db.Destinations.AddRange(toAdd);
//                db.SaveChanges();
//            }
//        }

//        // ---------------------------------------------------------------- Tours (+ images + itinerary)
//        private static void SeedTours(ApplicationDbContext db)
//        {
//            var dest = db.Destinations.ToDictionary(d => d.Slug, d => d.Id);
//            var act = db.ActivityTypes.ToDictionary(a => a.NameEn, a => a.Id);
//            var existing = db.Tours.Select(t => t.Slug).ToHashSet();

//            Tour T(string slug, string dSlug, string activity, TourType type, int days, decimal price,
//                   string titleEn, string titleFr, string sumEn, string sumFr, bool featured = false) => new Tour
//                   {
//                       Slug = slug,
//                       DestinationId = dest[dSlug],
//                       ActivityTypeId = act[activity],
//                       Type = type,
//                       DurationDays = days,
//                       PriceFrom = price,
//                       TitleEn = titleEn,
//                       TitleFr = titleFr,
//                       SummaryEn = sumEn,
//                       SummaryFr = sumFr,
//                       DescriptionEn = sumEn + " Fully private, with your own guide and driver, and flexible timing.",
//                       DescriptionFr = sumFr + " Entierement prive, avec votre guide et chauffeur, et un horaire flexible.",
//                       IsFeatured = featured,
//                       IsPrivate = true
//                   };

//            var candidates = new List<Tour>
//            {
//                T("cairo-alexandria-day-trip", "cairo", "Culture & History", TourType.DayTour, 1, 140,
//                  "Cairo to Alexandria Day Trip", "Excursion du Caire a Alexandrie",
//                  "Visit the Citadel of Qaitbay, Bibliotheca Alexandrina and the Mediterranean corniche.",
//                  "Visitez la citadelle de Qaitbay, la Bibliotheca Alexandrina et la corniche mediterraneenne."),

//                T("luxor-hot-air-balloon-sunrise", "luxor", "Adventure", TourType.DayTour, 1, 110,
//                  "Luxor Hot Air Balloon at Sunrise", "Montgolfiere a Louxor au lever du soleil",
//                  "Float over the Valley of the Kings and the Nile at dawn.",
//                  "Survolez la Vallee des Rois et le Nil a l'aube.", true),

//                T("cairo-luxor-aswan-6-days", "cairo", "Culture & History", TourType.FullPackage, 6, 980,
//                  "Cairo, Luxor & Aswan in 6 Days", "Le Caire, Louxor et Assouan en 6 jours",
//                  "The essential Egypt trip: pyramids, temples of Luxor and Nubian Aswan.",
//                  "L'essentiel de l'Egypte : pyramides, temples de Louxor et Assouan nubienne.", true),

//                T("classic-egypt-8-days", "cairo", "Culture & History", TourType.FullPackage, 8, 1450,
//                  "Classic Egypt in 8 Days", "Egypte classique en 8 jours",
//                  "Cairo, a Nile cruise and Luxor with hand-picked hotels and private transfers.",
//                  "Le Caire, une croisiere sur le Nil et Louxor avec hotels selectionnes et transferts prives.", true),

//                T("egypt-grand-tour-15-days", "cairo", "Culture & History", TourType.FullPackage, 15, 2450,
//                  "Egypt Grand Tour in 15 Days", "Grand tour d'Egypte en 15 jours",
//                  "From Cairo to Aswan, Siwa and the Red Sea: the complete Egypt experience.",
//                  "Du Caire a Assouan, Siwa et la mer Rouge : l'experience complete de l'Egypte."),

//                T("siwa-oasis-full-package", "siwa", "Desert & Oasis", TourType.FullPackage, 5, 760,
//                  "Siwa Oasis Escape in 5 Days", "Escapade a l'oasis de Siwa en 5 jours",
//                  "Salt lakes, Cleopatra's spring, Shali fortress and dune sunsets.",
//                  "Lacs sales, source de Cleopatre, forteresse de Shali et couchers de soleil sur les dunes."),

//                T("luxor-aswan-nile-cruise-5-days", "aswan", "Nile Sailing", TourType.NileCruise, 5, 620,
//                  "Luxor to Aswan Nile Cruise (4 Nights)", "Croisiere Louxor-Assouan (4 nuits)",
//                  "Sail between Luxor and Aswan, stopping at Edfu and Kom Ombo temples.",
//                  "Naviguez entre Louxor et Assouan avec escales aux temples d'Edfou et Kom Ombo.", true),

//                T("private-felucca-aswan-sunset", "aswan", "Nile Sailing", TourType.NileCruise, 1, 90,
//                  "Private Felucca at Sunset in Aswan", "Felouque privee au coucher du soleil a Assouan",
//                  "A quiet private felucca ride around Elephantine Island and the Botanical Garden.",
//                  "Une balade privee en felouque autour de l'ile Elephantine et du jardin botanique."),

//                T("siwa-oasis-desert-camping", "siwa", "Desert & Oasis", TourType.DesertCamping, 3, 380,
//                  "Siwa Desert Camping", "Camping dans le desert de Siwa",
//                  "Two nights under the stars in the Great Sand Sea with a Bedouin dinner.",
//                  "Deux nuits sous les etoiles dans la Grande Mer de Sable avec diner bedouin."),

//                T("bahariya-white-desert-camping", "bahariya", "Desert & Oasis", TourType.DesertCamping, 2, 290,
//                  "White Desert Camping from Bahariya", "Camping dans le desert Blanc depuis Bahariya",
//                  "Chalk formations, Crystal Mountain and a night camp in the White Desert.",
//                  "Formations de craie, Montagne de Cristal et nuit de camp dans le desert Blanc.", true),

//                T("hurghada-desert-jeep-safari", "hurghada", "Adventure", TourType.SafariByJeep, 1, 75,
//                  "Hurghada Desert Jeep Safari", "Safari en jeep dans le desert de Hurghada",
//                  "Jeep across the Eastern Desert with camel ride, Bedouin village and sunset tea.",
//                  "Jeep dans le desert oriental avec balade a dos de chameau, village bedouin et the au coucher du soleil."),

//                T("hurghada-beach-stay-7-nights", "hurghada", "Relaxation", TourType.BeachStay, 7, 890,
//                  "Red Sea Beach Stay (7 Nights)", "Sejour balneaire mer Rouge (7 nuits)",
//                  "A week of beach, snorkelling and optional dive days at a Red Sea resort.",
//                  "Une semaine de plage, snorkeling et plongee en option dans un resort de la mer Rouge.")
//            };

//            var newTours = candidates.Where(t => !existing.Contains(t.Slug)).ToList();
//            if (newTours.Count == 0) return;

//            var imgIndex = 0;
//            foreach (var t in newTours)
//            {
//                var a = (imgIndex % 3) + 1;
//                var b = ((imgIndex + 1) % 3) + 1;
//                imgIndex++;

//                t.CoverImageUrl = $"/images/tours/tour-{a}.jpeg";

//                t.Images.Add(new TourImage
//                {
//                    ImageUrl = $"/images/tours/tour-{a}.jpeg",
//                    AltTextEn = t.TitleEn,
//                    AltTextFr = t.TitleFr,
//                    DisplayOrder = 1
//                });
//                t.Images.Add(new TourImage
//                {
//                    ImageUrl = $"/images/tours/tour-{b}.jpeg",
//                    AltTextEn = t.TitleEn,
//                    AltTextFr = t.TitleFr,
//                    DisplayOrder = 2
//                });
//            }

//            AddItinerary(newTours, "classic-egypt-8-days", new[]
//            {
//                ("Arrival in Cairo", "Arrivee au Caire", "Airport pickup and hotel check-in.", "Accueil a l'aeroport et installation a l'hotel."),
//                ("Pyramids & Sphinx", "Pyramides et Sphinx", "Giza plateau, Sphinx and Saqqara.", "Plateau de Gizeh, Sphinx et Saqqara."),
//                ("Egyptian Museum & Old Cairo", "Musee egyptien et Vieux Caire", "Museum, Coptic Cairo and Khan el-Khalili.", "Musee, Caire copte et Khan el-Khalili."),
//                ("Fly to Luxor", "Vol vers Louxor", "Karnak and Luxor temples at sunset.", "Karnak et temple de Louxor au coucher du soleil."),
//                ("West Bank of Luxor", "Rive ouest de Louxor", "Valley of the Kings and Hatshepsut Temple.", "Vallee des Rois et temple d'Hatchepsout."),
//                ("Nile cruise to Edfu", "Croisiere vers Edfou", "Board the cruise and visit Edfu temple.", "Embarquement et visite du temple d'Edfou."),
//                ("Kom Ombo & Aswan", "Kom Ombo et Assouan", "Kom Ombo temple then felucca in Aswan.", "Temple de Kom Ombo puis felouque a Assouan."),
//                ("Departure", "Depart", "Transfer to the airport.", "Transfert a l'aeroport.")
//            });

//            AddItinerary(newTours, "luxor-aswan-nile-cruise-5-days", new[]
//            {
//                ("Embark in Luxor", "Embarquement a Louxor", "Welcome drink and Karnak visit.", "Cocktail de bienvenue et visite de Karnak."),
//                ("Valley of the Kings", "Vallee des Rois", "West Bank sightseeing, sail to Esna.", "Rive ouest, navigation vers Esna."),
//                ("Edfu & Kom Ombo", "Edfou et Kom Ombo", "Horus temple by carriage, Kom Ombo at dusk.", "Temple d'Horus en calèche, Kom Ombo au crepuscule."),
//                ("Aswan", "Assouan", "High Dam, Philae Temple and felucca ride.", "Haut Barrage, temple de Philae et felouque."),
//                ("Disembark", "Debarquement", "Breakfast and transfer.", "Petit-dejeuner et transfert.")
//            });

//            AddItinerary(newTours, "siwa-oasis-desert-camping", new[]
//            {
//                ("Drive to Siwa", "Route vers Siwa", "Cross the Western Desert, visit Shali fortress.", "Traversee du desert occidental, forteresse de Shali."),
//                ("Great Sand Sea", "Grande Mer de Sable", "Dune driving, salt lakes and desert camp.", "Dunes, lacs sales et camp dans le desert."),
//                ("Return", "Retour", "Cleopatra's spring, then drive back.", "Source de Cleopatre puis retour.")
//            });

//            AddItinerary(newTours, "bahariya-white-desert-camping", new[]
//            {
//                ("Black Desert & Crystal Mountain", "Desert Noir et Montagne de Cristal", "Drive from Bahariya and camp in the White Desert.", "Route depuis Bahariya et camp dans le desert Blanc."),
//                ("White Desert sunrise", "Lever de soleil dans le desert Blanc", "Explore the chalk rocks and return.", "Exploration des roches de craie et retour.")
//            });

//            db.Tours.AddRange(newTours);
//            db.SaveChanges();
//        }

//        private static void AddItinerary(List<Tour> tours, string slug,
//            (string TitleEn, string TitleFr, string DescEn, string DescFr)[] days)
//        {
//            var tour = tours.FirstOrDefault(t => t.Slug == slug);
//            if (tour == null) return;

//            for (int i = 0; i < days.Length; i++)
//            {
//                tour.Itinerary.Add(new TourItinerary
//                {
//                    DayNumber = i + 1,
//                    TitleEn = days[i].TitleEn,
//                    TitleFr = days[i].TitleFr,
//                    DescriptionEn = days[i].DescEn,
//                    DescriptionFr = days[i].DescFr
//                });
//            }
//        }

//        // ---------------------------------------------------------------- Blog
//        private static void SeedBlog(ApplicationDbContext db)
//        {
//            if (!db.BlogCategories.Any())
//            {
//                db.BlogCategories.AddRange(
//                    new BlogCategory { NameEn = "Travel Tips", NameFr = "Conseils de voyage", Slug = "travel-tips" },
//                    new BlogCategory { NameEn = "Egypt Guides", NameFr = "Guides de l'Egypte", Slug = "egypt-guides" },
//                    new BlogCategory { NameEn = "Food & Culture", NameFr = "Cuisine et culture", Slug = "food-culture" }
//                );
//                db.SaveChanges();
//            }

//            if (db.BlogPosts.Any()) return;

//            var cat = db.BlogCategories.ToDictionary(c => c.Slug, c => c.Id);
//            var now = DateTime.UtcNow;

//            db.BlogPosts.AddRange(
//                new BlogPost
//                {
//                    TitleEn = "Best Time to Visit Egypt",
//                    TitleFr = "Meilleure periode pour visiter l'Egypte",
//                    Slug = "best-time-to-visit-egypt",
//                    SummaryEn = "Month-by-month advice on weather and crowds.",
//                    SummaryFr = "Conseils mois par mois sur la meteo et l'affluence.",
//                    ContentEn = "October to April is the most comfortable season for sightseeing. Summers are hot in Upper Egypt, but perfect for the Red Sea coast.",
//                    ContentFr = "D'octobre a avril, c'est la saison la plus agreable pour les visites. L'ete est chaud en Haute-Egypte mais ideal pour la mer Rouge.",
//                    BlogCategoryId = cat["travel-tips"],
//                    Author = "Tajan Team",
//                    IsPublished = true,
//                    PublishedAt = now.AddDays(-20)
//                },
//                new BlogPost
//                {
//                    TitleEn = "First Timer's Guide to Luxor",
//                    TitleFr = "Guide de Louxor pour les premiers visiteurs",
//                    Slug = "first-timers-guide-to-luxor",
//                    SummaryEn = "What to see on the East and West Banks.",
//                    SummaryFr = "Ce qu'il faut voir sur les rives est et ouest.",
//                    ContentEn = "Start early at the Valley of the Kings, then spend the afternoon at Karnak and the Luxor Temple after sunset.",
//                    ContentFr = "Commencez tot a la Vallee des Rois, puis passez l'apres-midi a Karnak et visitez le temple de Louxor apres le coucher du soleil.",
//                    BlogCategoryId = cat["egypt-guides"],
//                    Author = "Tajan Team",
//                    IsPublished = true,
//                    PublishedAt = now.AddDays(-12)
//                },
//                new BlogPost
//                {
//                    TitleEn = "Egyptian Dishes You Must Try",
//                    TitleFr = "Plats egyptiens a gouter absolument",
//                    Slug = "egyptian-dishes-you-must-try",
//                    SummaryEn = "From koshari to molokhia, a tasting guide.",
//                    SummaryFr = "Du koshari a la molokhia, un guide de degustation.",
//                    ContentEn = "Koshari, ful medames, molokhia and fresh baladi bread are the foundations of the Egyptian table.",
//                    ContentFr = "Koshari, ful medames, molokhia et pain baladi frais sont les bases de la table egyptienne.",
//                    BlogCategoryId = cat["food-culture"],
//                    Author = "Tajan Team",
//                    IsPublished = true,
//                    PublishedAt = now.AddDays(-5)
//                },
//                new BlogPost
//                {
//                    TitleEn = "Packing List for the Desert (Draft)",
//                    TitleFr = "Liste de bagages pour le desert (brouillon)",
//                    Slug = "packing-list-for-the-desert",
//                    SummaryEn = "What to bring for a night in the White Desert.",
//                    SummaryFr = "Quoi emporter pour une nuit dans le desert Blanc.",
//                    ContentEn = "Layers, a warm jacket, sun protection and a headlamp.",
//                    ContentFr = "Des couches, une veste chaude, une protection solaire et une lampe frontale.",
//                    BlogCategoryId = cat["travel-tips"],
//                    Author = "Tajan Team",
//                    IsPublished = false
//                }
//            );
//            db.SaveChanges();
//        }

//        // ---------------------------------------------------------------- Testimonials
//        private static void SeedTestimonials(ApplicationDbContext db)
//        {
//            if (db.Testimonials.Any()) return;

//            var tours = db.Tours.ToDictionary(t => t.Slug, t => t.Id);
//            int? Id(string slug) => tours.TryGetValue(slug, out var id) ? id : null;

//            db.Testimonials.AddRange(
//                new Testimonial
//                {
//                    ClientName = "Sophie Martin",
//                    ClientCountry = "France",
//                    Rating = 5,
//                    QuoteEn = "Our guide made the pyramids feel alive. Everything was perfectly organised.",
//                    QuoteFr = "Notre guide a rendu les pyramides vivantes. Tout etait parfaitement organise.",
//                    TourId = Id("pyramids-egyptian-museum-tour"),
//                    IsFeatured = true,
//                    IsApproved = true,
//                    DisplayOrder = 1
//                },
//                new Testimonial
//                {
//                    ClientName = "James Carter",
//                    ClientCountry = "United States",
//                    Rating = 5,
//                    QuoteEn = "The hot air balloon at sunrise was the highlight of our trip.",
//                    QuoteFr = "La montgolfiere au lever du soleil a ete le point fort de notre voyage.",
//                    TourId = Id("luxor-hot-air-balloon-sunrise"),
//                    IsFeatured = true,
//                    IsApproved = true,
//                    DisplayOrder = 2
//                },
//                new Testimonial
//                {
//                    ClientName = "Claire Dubois",
//                    ClientCountry = "France",
//                    Rating = 5,
//                    QuoteEn = "Siwa was magical. Camping under the stars is unforgettable.",
//                    QuoteFr = "Siwa etait magique. Camper sous les etoiles est inoubliable.",
//                    TourId = Id("siwa-oasis-desert-camping"),
//                    IsFeatured = true,
//                    IsApproved = true,
//                    DisplayOrder = 3
//                },
//                new Testimonial
//                {
//                    ClientName = "Michael Brown",
//                    ClientCountry = "Canada",
//                    Rating = 4,
//                    QuoteEn = "Smooth Nile cruise, great food and a very friendly crew.",
//                    QuoteFr = "Croisiere sur le Nil agreable, excellente cuisine et equipage tres sympathique.",
//                    TourId = Id("luxor-aswan-nile-cruise-5-days"),
//                    IsFeatured = false,
//                    IsApproved = true,
//                    DisplayOrder = 4
//                },
//                new Testimonial
//                {
//                    ClientName = "Emma Wilson",
//                    ClientCountry = "United Kingdom",
//                    Rating = 5,
//                    QuoteEn = "Flexible payment and clear communication made booking stress-free.",
//                    QuoteFr = "Le paiement flexible et la communication claire ont rendu la reservation sans stress.",
//                    IsFeatured = false,
//                    IsApproved = true,
//                    DisplayOrder = 5
//                },
//                new Testimonial
//                {
//                    ClientName = "Lucas Meyer",
//                    ClientCountry = "Germany",
//                    Rating = 4,
//                    QuoteEn = "Waiting for approval.",
//                    QuoteFr = "En attente d'approbation.",
//                    IsFeatured = false,
//                    IsApproved = false,
//                    DisplayOrder = 6
//                }
//            );
//            db.SaveChanges();
//        }

//        // ---------------------------------------------------------------- FAQs
//        private static void SeedFaqs(ApplicationDbContext db)
//        {
//            if (db.FAQs.Any()) return;

//            FAQ F(int order, string category, string qEn, string qFr, string aEn, string aFr) => new FAQ
//            {
//                DisplayOrder = order,
//                Category = category,
//                QuestionEn = qEn,
//                QuestionFr = qFr,
//                AnswerEn = aEn,
//                AnswerFr = aFr
//            };

//            db.FAQs.AddRange(
//                F(1, "Booking", "How do I book a tour?", "Comment reserver un circuit ?",
//                  "Send us an inquiry and our team will confirm details within 24 hours.",
//                  "Envoyez-nous une demande et notre equipe confirmera les details sous 24 heures."),
//                F(2, "Payment", "How does payment work?", "Comment fonctionne le paiement ?",
//                  "Pay 50% online to confirm and the rest upon arrival.",
//                  "Payez 50% en ligne pour confirmer et le reste a l'arrivee."),
//                F(3, "Payment", "Is online payment secure?", "Le paiement en ligne est-il securise ?",
//                  "Yes, payments are protected with trusted secure systems.",
//                  "Oui, les paiements sont proteges par des systemes securises."),
//                F(4, "Booking", "Are the tours private?", "Les circuits sont-ils prives ?",
//                  "All our tours are private and designed around your group.",
//                  "Tous nos circuits sont prives et concus autour de votre groupe."),
//                F(5, "Cancellation", "What is your cancellation policy?", "Quelle est votre politique d'annulation ?",
//                  "Free cancellation up to 7 days before departure; later cancellations follow our clear terms.",
//                  "Annulation gratuite jusqu'a 7 jours avant le depart ; ensuite selon nos conditions claires."),
//                F(6, "Travel", "Do I need a visa for Egypt?", "Ai-je besoin d'un visa pour l'Egypte ?",
//                  "Most nationalities can get a visa on arrival or an e-visa. Please check before you travel.",
//                  "La plupart des nationalites peuvent obtenir un visa a l'arrivee ou un e-visa. Verifiez avant de partir."),
//                F(7, "Travel", "Is Egypt safe for tourists?", "L'Egypte est-elle sure pour les touristes ?",
//                  "Yes. Our guides and drivers are licensed and travel with you the entire trip.",
//                  "Oui. Nos guides et chauffeurs sont agrees et vous accompagnent pendant tout le voyage."),
//                F(8, "Travel", "Can you arrange custom itineraries?", "Pouvez-vous creer des itineraires sur mesure ?",
//                  "Yes, tell us your dates and interests and we will build a plan for you.",
//                  "Oui, indiquez-nous vos dates et centres d'interet et nous creerons un programme pour vous.")
//            );
//            db.SaveChanges();
//        }

//        // ---------------------------------------------------------------- Inquiries
//        private static void SeedInquiries(ApplicationDbContext db)
//        {
//            if (db.Inquiries.Any()) return;

//            var tours = db.Tours.ToDictionary(t => t.Slug, t => t.Id);
//            int? Id(string slug) => tours.TryGetValue(slug, out var id) ? id : null;
//            var today = DateTime.UtcNow.Date;

//            db.Inquiries.AddRange(
//                new Inquiry
//                {
//                    FullName = "Anna Schmidt",
//                    Email = "anna.schmidt@example.com",
//                    PhoneNumber = "+49 151 2345 6789",
//                    PreferredDate = today.AddDays(45),
//                    GroupSize = 2,
//                    AdditionalNotes = "Honeymoon trip, we prefer a slower pace.",
//                    Type = InquiryType.TourInquiry,
//                    TourId = Id("classic-egypt-8-days"),
//                    Status = InquiryStatus.New
//                },
//                new Inquiry
//                {
//                    FullName = "Pierre Laurent",
//                    Email = "pierre.laurent@example.com",
//                    PhoneNumber = "+33 6 12 34 56 78",
//                    PreferredDate = today.AddDays(30),
//                    GroupSize = 4,
//                    AdditionalNotes = "Travelling with two teenagers.",
//                    Type = InquiryType.TourInquiry,
//                    TourId = Id("cairo-luxor-aswan-6-days"),
//                    Status = InquiryStatus.Contacted
//                },
//                new Inquiry
//                {
//                    FullName = "David Johnson",
//                    Email = "david.johnson@example.com",
//                    PhoneNumber = "+1 415 555 0142",
//                    PreferredDate = today.AddDays(60),
//                    GroupSize = 6,
//                    AdditionalNotes = "Group of friends, interested in desert camping.",
//                    Type = InquiryType.TourInquiry,
//                    TourId = Id("bahariya-white-desert-camping"),
//                    Status = InquiryStatus.InProgress
//                },
//                new Inquiry
//                {
//                    FullName = "Maria Rossi",
//                    Email = "maria.rossi@example.com",
//                    PhoneNumber = "+39 333 123 4567",
//                    PreferredDate = today.AddDays(90),
//                    GroupSize = 3,
//                    AdditionalNotes = "Would like Cairo, Siwa and the Red Sea in 10 days.",
//                    Type = InquiryType.CustomItinerary,
//                    Status = InquiryStatus.Confirmed
//                },
//                new Inquiry
//                {
//                    FullName = "Olivia Green",
//                    Email = "olivia.green@example.com",
//                    PhoneNumber = "+44 7700 900123",
//                    GroupSize = 1,
//                    AdditionalNotes = "Do you offer solo traveller discounts?",
//                    Type = InquiryType.GeneralContact,
//                    Status = InquiryStatus.Closed
//                }
//            );
//            db.SaveChanges();
//        }

//        // ---------------------------------------------------------------- Contact messages
//        private static void SeedContactMessages(ApplicationDbContext db)
//        {
//            if (db.ContactMessages.Any()) return;

//            db.ContactMessages.AddRange(
//                new ContactMessage
//                {
//                    FullName = "Thomas Becker",
//                    Email = "thomas.becker@example.com",
//                    PhoneNumber = "+49 170 1234567",
//                    Subject = "Question about Nile cruises",
//                    Message = "Which cruise dates are available in November?",
//                    IsRead = false
//                },
//                new ContactMessage
//                {
//                    FullName = "Isabelle Moreau",
//                    Email = "isabelle.moreau@example.com",
//                    Subject = "Partnership proposal",
//                    Message = "We are a small travel agency in Lyon and would like to discuss a partnership.",
//                    IsRead = true
//                },
//                new ContactMessage
//                {
//                    FullName = "Ahmed Hassan",
//                    Email = "ahmed.hassan@example.com",
//                    PhoneNumber = "+20 100 123 4567",
//                    Subject = "Group discount",
//                    Message = "Do you offer discounts for groups of more than 10 people?",
//                    IsRead = false
//                }
//            );
//            db.SaveChanges();
//        }
//    }
//}