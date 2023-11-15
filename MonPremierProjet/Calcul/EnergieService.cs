// fichier: EnergieService.cs
using MonPremierProjet.Data;
using MonPremierProjet.DTO;
using MonPremierProjet.Enum;
using MonPremierProjet.Models;
using System.Runtime.CompilerServices;

namespace MonPremierProjet.Calcul
{
    public class EnergieService : IEnergieService
    {
        
        private readonly IAjustementEnergieRepository _repository;
        
            
        
        public EnergieService(IAjustementEnergieRepository repository)
        {
            _repository = repository;
            
        }

        // Constantes
        private readonly decimal DuHeureF3 = 195;
        private readonly decimal DuHeureF2 = 0;
        private readonly decimal DuHeureChromojet = 0;
        private readonly decimal DuHeureTuffter = 3666;

        

        private async Task<decimal> CalculerElectKwh(Section section)
        {
            // await car FindOne est un task
            var electriqueBeamer = await _repository.FindOne(a => a.NomAjustement == "ElectriqueBeamer");
            var electriqueTable = await _repository.FindOne(a => a.NomAjustement == "ElectriqueTable");
            var electriqueCentreAdm = await _repository.FindOne(a => a.NomAjustement == "ElectriqueCentreAdm");
            var repartitionDistribution = await _repository.FindOne(a => a.NomAjustement == "RepartitionDistribution");
            var repartitionSampling = await _repository.FindOne(a => a.NomAjustement == "RepartitionSampling");

           

            decimal U3ChromojetElectKwh = DuHeureChromojet * 300; // enlever /3600
            decimal U3LigneFin2ElectKwh = DuHeureF2 * 200; // enlever /3600
            decimal U3LigneFin3ElectKwh = DuHeureF3 * 200; // enlever /3600
            decimal U3ToufftageElectKwh = DuHeureTuffter * 20;  // enlever /3600
            decimal ElectriciteConnu = U3ChromojetElectKwh + U3LigneFin2ElectKwh + U3LigneFin3ElectKwh + U3ToufftageElectKwh;

            


            if(electriqueBeamer == null || electriqueTable == null || electriqueCentreAdm == null || repartitionDistribution == null || repartitionSampling == null)
                throw new Exception("AjustementEnergie introuvable");
                
                // forumule pour calculer ElectKwh pour chaque section
            return section switch
            {
        
                Section.AdminConsTotal => 438190m * electriqueCentreAdm.Valeur / 100,
                    
                Section.U4ConsTotal => 31000, // Valeur constante

                Section.U3ConsTotal => 438190m - await CalculerElectKwh(Section.AdminConsTotal) - await CalculerElectKwh(Section.U4ConsTotal),

                Section.U3Beamer => ElectriciteConnu * electriqueBeamer.Valeur / 100,

                 Section.U3TableInspect => ElectriciteConnu * electriqueTable.Valeur / 100,

                Section.U3ConsPro => ElectriciteConnu + await CalculerElectKwh(Section.U3Beamer) + await CalculerElectKwh(Section.U3TableInspect),

                Section.U3Distribution =>(await CalculerElectKwh(Section.U3ConsTotal) - await CalculerElectKwh(Section.U3ConsPro)) * repartitionDistribution.Valeur / 100,

                Section.U3Sampling => (await CalculerElectKwh(Section.U3ConsTotal) - await CalculerElectKwh(Section.U3ConsPro)) * repartitionSampling.Valeur / 100,

                Section.U3ConsFiPro => (await CalculerElectKwh(Section.U3ConsTotal) - await CalculerElectKwh(Section.U3ConsPro )) - (await CalculerElectKwh(Section.U3Distribution) + await CalculerElectKwh(Section.U3Sampling)),

                Section.U3Touffetage => U3ToufftageElectKwh,
                Section.U3ConsCh => 0, // Valeur absente
                Section.U3LigneFin2 => U3LigneFin2ElectKwh, // Valeur absente
                Section.U3LigneFin3 => U3LigneFin3ElectKwh,
                Section.U3Chromojet => U3ChromojetElectKwh, // Valeur absente
                _ => 0,
                };
                    
        }

        private async Task<decimal> CalculerGazM3(Section section)
        {
            // Constantes et récupération des valeurs nécessaires
            var repartitionDistribution = await _repository.FindOne(a => a.NomAjustement == "RepartitionDistribution");
            var repartitionSampling = await _repository.FindOne(a => a.NomAjustement == "RepartitionSampling");
            decimal gazParHeureF3 = 137;
            decimal gazParHeureF2 = 130;
            decimal gazParHeureChromojet = 251;
            

            decimal gazF3 = DuHeureF3 * gazParHeureF3;
            decimal gazF2 = DuHeureF2 * gazParHeureF2;
            decimal gazChromojet = DuHeureChromojet * gazParHeureChromojet;
            decimal gazConnu = gazF3 + gazF2 + gazChromojet;

            if ( repartitionDistribution == null || repartitionSampling == null)
                throw new Exception("AjustementEnergie introuvable");

            return section switch
            {
                Section.AdminConsTotal => 0, // AdminConsTotal n'utilise pas de gaz dans vos formules
                Section.U4ConsTotal => 0, // U4ConsTotal n'utilise pas de gaz dans vos formules
                Section.U3ConsPro => 33041,
                Section.U3ConsCh => 18269,
                Section.U3ConsTotal => await CalculerGazM3(Section.U3ConsPro) + await CalculerGazM3(Section.U3ConsCh),
                Section.U3Distribution => await CalculerGazM3(Section.U3ConsCh) * (repartitionDistribution.Valeur / 100),
                Section.U3Sampling => await CalculerGazM3(Section.U3ConsCh) * (repartitionSampling.Valeur / 100),
                Section.U3ConsFiPro => await CalculerGazM3(Section.U3ConsPro) - gazConnu,
                Section.U3LigneFin3 => gazConnu > 0 ? gazF3 + await CalculerGazM3(Section.U3ConsFiPro) * (gazF3 / gazConnu) : 0,

                Section.U3LigneFin2 => gazConnu > 0 ? gazF2 + await CalculerGazM3(Section.U3ConsFiPro) * (gazF2 / gazConnu) : 0,

                Section.U3Chromojet => gazConnu > 0 ? gazChromojet + await CalculerGazM3(Section.U3ConsFiPro) * (gazChromojet / gazConnu) : 0,

                Section.U3Beamer => 0, // Valeur absente
                Section.U3TableInspect => 0, // Valeur absente
                Section.U3Touffetage => 0, // Valeur absente
                _ => 0,
            };
        }
        private async Task<decimal> CalculerEauM3(Section section)
        {
            // Récupération des valeurs nécessaires de la table ajustementenergie
            var eauFixe = await  _repository.FindOne(a => a.NomAjustement == "EauFixe");
            var eauProduction = await _repository.FindOne(a => a.NomAjustement == "EauProduction");
            var eauTouffetage = await _repository.FindOne(a => a.NomAjustement == "EauTouffetage");
            var eauL2 = await _repository.FindOne(a => a.NomAjustement == "EauL2");
            var eauL3 = await _repository.FindOne(a => a.NomAjustement == "EauL3");
            var eauChromojet =  await _repository.FindOne(a => a.NomAjustement == "EauChromojet");
            var repartitionDistribution = await _repository.FindOne(a => a.NomAjustement == "RepartitionDistribution");
            var repartitionSampling = await _repository.FindOne(a => a.NomAjustement == "RepartitionSampling");

            decimal totalEauM3 = 1084;

            if (eauFixe == null || eauProduction == null || eauTouffetage == null || eauL2 == null || eauL3 == null || eauChromojet == null || repartitionSampling == null || repartitionDistribution == null)
                throw new Exception("AjustementEnergie introuvable");

            return section switch
            {
                Section.AdminConsTotal => 0, // AdminConsTotal n'utilise pas d'eau dans les formules
                Section.U4ConsTotal => 0, // U4ConsTotal n'utilise pas d'eau dans les formules
                Section.U3ConsTotal => totalEauM3,
                Section.U3ConsPro => totalEauM3 * eauProduction.Valeur / 100,
                Section.U3Touffetage => await CalculerEauM3(Section.U3ConsPro) * eauTouffetage.Valeur / 100,
                Section.U3LigneFin2 => await CalculerEauM3(Section.U3ConsPro) * eauL2.Valeur / 100,
                Section.U3LigneFin3 => await CalculerEauM3(Section.U3ConsPro) * eauL3.Valeur / 100,
                Section.U3Chromojet => await CalculerEauM3(Section.U3ConsPro) * eauChromojet.Valeur/ 100,
                Section.U3Distribution => await CalculerEauM3(Section.U3ConsTotal) * (eauFixe.Valeur / 100) * (repartitionDistribution.Valeur / 100),
                Section.U3Sampling => await CalculerEauM3(Section.U3ConsTotal) * (eauFixe.Valeur / 100) * (repartitionSampling.Valeur/ 100),
                Section.U3ConsCh => 0, // U3ConsCh n'utilise pas d'eau dans les formules
                Section.U3ConsFiPro => (await CalculerEauM3(Section.U3ConsTotal) * eauFixe.Valeur / 100) -
                        (await CalculerEauM3(Section.U3Distribution) + await CalculerEauM3(Section.U3Sampling)),
                Section.U3Beamer => 0, // U3Beamer n'utilise pas d'eau dans les formules

                _ => 0,
            };
        }

       

        

        public async Task<List<CoutEnergieDto>> GetCoutEnergies()
        {
            var test = await _repository.FindAll();    
            var coutEnergies = new List<CoutEnergieDto>();

            foreach (Section section in System.Enum.GetValues<Section>())
            {
                var coutEnergie = new CoutEnergieDto();
                coutEnergie.ElectDollar = await CalculateDollars(section, EnergyType.Electric);
                
                coutEnergie.GazDollar = await CalculateDollars(section, EnergyType.Gas);
                coutEnergie.EauDollar = await CalculateDollars(section, EnergyType.Water);
                coutEnergie.ElectKwh = await CalculerElectKwh(section);
                coutEnergie.GazM3 = await CalculerGazM3(section);
                coutEnergie.EauM3 = await CalculerEauM3(section);
            }
            return coutEnergies;

        }

        private async Task<decimal> CalculateDollars(Section section, EnergyType energyType)
        {
            if (energyType == EnergyType.Electric)
            {
                var electricite = await _repository.FindOne(a => a.NomAjustement == "DollarsElectricite");
                if (electricite == null) return 0;
                   return await CalculerElectKwh(section) * electricite.Valeur;
            }
            if (energyType == EnergyType.Gas)
            {
                var gas = await _repository.FindOne(a => a.NomAjustement == "DollarsGaz");
                if (gas == null) return 0;
                return await CalculerGazM3(section) * gas.Valeur;
            }
            if (energyType == EnergyType.Water)
            {
                var eau = await _repository.FindOne(a => a.NomAjustement == "DollarsEau");
                if (eau == null) return 0;
                return await CalculerEauM3(section) * eau.Valeur;
            }
            
            
                throw new Exception("Type d'énergie invalide");
            
           
        }
    }



}
