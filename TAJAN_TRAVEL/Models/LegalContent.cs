using TAJAN_TRAVEL.Models.ViewModels;

namespace TAJAN_TRAVEL.Models
{
    /// <summary>Text of the Terms of Service and the Privacy Policy (English / French).</summary>
    public static class LegalContent
    {
        private static LegalItem[] I(params string[] texts) => texts.Select(t => new LegalItem(t)).ToArray();

        // ------------------------------------------------------------------ TERMS
        public static LegalPageViewModel Terms(bool fr) => fr
            ? new LegalPageViewModel
            {
                Title = "Conditions d'utilisation",
                Updated = "Dernière mise à jour : octobre 2026",
                Intro = "Bienvenue sur notre site touristique. Veuillez lire attentivement les conditions d'utilisation suivantes avant d'utiliser le site. En accédant à notre site ou en nous contactant par son intermédiaire, vous acceptez d'être lié par ces conditions.",
                Sections =
                {
                    new("1. Nature des services fournis", null, new[]
                    {
                        new LegalItem("Le site présente des programmes, services et voyages touristiques, axés actuellement sur les voyages à destination de l'Égypte, avec des perspectives d'expansion future pour organiser des voyages entre différents pays du monde."),
                        new LegalItem("Aucune transaction de paiement électronique direct ni achat automatique en ligne n'est effectué sur le site. Les demandes de réservation et de renseignements sont soumises via les moyens de contact indiqués (tels que l'e-mail ou WhatsApp).", "Le site est destiné uniquement à la consultation et aux demandes d'information :")
                    }),
                    new("2. Réservations et transactions financières", null, I(
                        "La navigation sur le site et la consultation des programmes touristiques ne constituent pas une réservation confirmée.",
                        "Les confirmations de réservation, les conditions de paiement, les politiques d'annulation et les détails du voyage sont convenus de manière indépendante lors d'une communication directe entre le visiteur et notre équipe par e-mail ou WhatsApp.")),
                    new("3. Propriété intellectuelle", null, I(
                        "Tous les textes, images, logos et designs affichés sur le site sont notre propriété exclusive (ou sous licence d'utilisation). La copie ou la republication de toute partie du contenu du site sans autorisation écrite préalable est strictement interdite.")),
                    new("4. Utilisation du site", null, I(
                        "Vous vous engagez à ne pas utiliser le site à des fins illégales ou pour tenter de nuire à l'infrastructure du site.",
                        "Le site ne nécessite pas la création de comptes personnels et ne permet pas la publication de contenu par les utilisateurs.")),
                    new("5. Limitation de responsabilité", null, I(
                        "Nous nous efforçons de fournir des informations précises et des détails spécifiques concernant les programmes touristiques et les prix. Cependant, certains détails peuvent changer en fonction de la disponibilité des hôtels ou des vols, ou en raison de circonstances indépendantes de notre volonté. Les détails définitifs sont officiellement confirmés lors de la communication directe et avant la conclusion du contrat."))
                }
            }
            : new LegalPageViewModel
            {
                Title = "Terms of Service",
                Updated = "Last updated: October 2026",
                Intro = "Welcome to our tourism website. Please read the following Terms of Service carefully before using the website. By accessing or using our website, or contacting us through it, you agree to be bound by these terms.",
                Sections =
                {
                    new("1. Nature of Provided Services", null, new[]
                    {
                        new LegalItem("The website offers and displays tourism programs, services, and trips, currently focusing on inbound travel to Egypt, with plans for future expansion to organize trips between various countries worldwide."),
                        new LegalItem("No direct electronic payment transactions or automated online purchases are processed through the website. Booking requests and inquiries are submitted via the specified contact methods (such as Email or WhatsApp).", "The website is for display and inquiry purposes only:")
                    }),
                    new("2. Bookings and Financial Transactions", null, I(
                        "Browsing the website and viewing tourism programs does not constitute a confirmed booking.",
                        "Booking confirmations, payment terms, cancellation policies, and trip details are agreed upon independently through direct communication between the visitor and our team via Email or WhatsApp.")),
                    new("3. Intellectual Property", null, I(
                        "All texts, images, logos, and designs displayed on the website are our exclusive property (or licensed for use). Copying or republishing any part of the website's content without prior written permission is strictly prohibited.")),
                    new("4. Website Usage", null, I(
                        "You agree not to use the website for any unlawful purposes or attempt to harm the website's infrastructure.",
                        "The website does not require creating personal accounts, nor does it allow user-generated content.")),
                    new("5. Limitation of Liability", null, I(
                        "We strive to provide accurate information and specific details regarding tourism programs and prices. However, certain details may change based on hotel/flight availability or circumstances beyond our control. Final details are officially confirmed during direct communication and prior to contract agreement."))
                }
            };

        // ---------------------------------------------------------------- PRIVACY
        public static LegalPageViewModel Privacy(bool fr) => fr
            ? new LegalPageViewModel
            {
                Title = "Politique de confidentialité",
                Updated = "Dernière mise à jour : octobre 2026",
                Intro = "Nous accordons une grande importance à la confidentialité de nos visiteurs et nous nous engageons à protéger vos données personnelles conformément aux normes de sécurité et de confidentialité les plus élevées.",
                Sections =
                {
                    new("1. Données que nous collectons",
                        "Nous collectons les données personnelles que vous nous fournissez volontairement lorsque vous nous contactez pour une demande d'information ou pour solliciter un service touristique, notamment :",
                        I("Nom complet.", "Numéro de téléphone / WhatsApp.", "Adresse e-mail.", "Nationalité.")),
                    new("2. Comment nous utilisons vos données",
                        "Nous utilisons les données collectées uniquement à des fins limitées, à savoir :",
                        I("Communiquer avec vous et répondre à vos demandes concernant les programmes touristiques.",
                          "Organiser et coordonner les voyages et réservations en fonction de votre nationalité et de vos exigences de voyage.",
                          "Améliorer nos services et l'expérience utilisateur.")),
                    new("3. Partage des données avec des tiers", null, I(
                        "Nous ne vendons, ne louons ni ne commercialisons vos données personnelles à des tiers.",
                        "Nous pouvons partager uniquement les données nécessaires (telles que le nom et la nationalité) avec des prestataires de services touristiques (tels que des hôtels, des compagnies aériennes ou des entreprises de transport) uniquement si un voyage est directement convenu et confirmé avec vous dans le but de finaliser la réservation.")),
                    new("4. Fichiers témoins (cookies)", null, I(
                        "Le site peut utiliser des cookies essentiels pour améliorer l'expérience de navigation et analyser les performances du site. Vous pouvez contrôler ou désactiver les cookies via les paramètres de votre navigateur.")),
                    new("5. Sécurité des données", null, I(
                        "Nous appliquons des mesures de sécurité appropriées pour protéger vos données contre tout accès, modification ou divulgation non autorisés.")),
                    new("6. Vos droits et nous contacter", null, I(
                        "Vous avez le droit, à tout moment, de demander l'accès à vos données personnelles, leur rectification ou leur suppression complète de nos registres de communication.",
                        "Pour toute question concernant cette politique de confidentialité ou vos données personnelles, vous pouvez nous contacter via l'e-mail ou le numéro WhatsApp indiqués sur le site."))
                }
            }
            : new LegalPageViewModel
            {
                Title = "Privacy Policy",
                Updated = "Last updated: October 2026",
                Intro = "We attach great importance to the privacy of our visitors and are committed to protecting your personal data in accordance with the highest security and privacy standards.",
                Sections =
                {
                    new("1. Data We Collect",
                        "We collect personal data that you voluntarily provide to us when contacting us for inquiries or requesting a tourism service, which includes:",
                        I("Full name.", "Phone / WhatsApp number.", "Email address.", "Nationality.")),
                    new("2. How We Use Your Data",
                        "We use the collected data strictly for limited purposes, which are:",
                        I("Communicating with you and responding to your inquiries regarding tourism programs.",
                          "Arranging and coordinating trips and bookings according to your nationality and travel requirements.",
                          "Improving our services and user experience.")),
                    new("3. Sharing Data with Third Parties", null, I(
                        "We do not sell, rent, or trade your personal data with any third parties.",
                        "We may share necessary data only (such as name and nationality) with tourism service providers (such as hotels, airlines, or transport companies) solely if a trip is directly agreed upon and confirmed with you for the purpose of completing the reservation.")),
                    new("4. Cookies", null, I(
                        "The website may use essential cookies to enhance the browsing experience and analyze website performance. You can control or disable cookies through your browser settings.")),
                    new("5. Data Security", null, I(
                        "We implement appropriate security measures to protect your data from unauthorized access, alteration, or disclosure.")),
                    new("6. Your Rights and Contact Us", null, I(
                        "You have the right at any time to request access to, correction of, or complete deletion of your personal data from our communication records.",
                        "For any inquiries regarding this Privacy Policy or your personal data, you can contact us via the Email or WhatsApp provided on the website."))
                }
            };
    }
}
