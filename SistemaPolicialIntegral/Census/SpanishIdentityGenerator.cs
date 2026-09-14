using System;

namespace SistemaPolicialIntegral.Census
{
    public class SpanishIdentity
    {
        public string Nombre;
        public string PrimerApellido;
        public string SegundoApellido;
        public string NombreCompleto;
        public string DniNumero;
        public DateTime FechaNacimiento;
        public string Direccion;
        public bool TieneAntecedentes;
        public int PuntosDGT;
    }

    public static class SpanishIdentityGenerator
    {
        private static Random random = new Random();

        private static string[] nombresMasculinos = new string[] 
        {
            "Alejandro", "Antonio", "Carlos", "Daniel", "David", "Diego", "Eduardo", "Enrique", "Felipe", "Fernando", 
            "Francisco", "Gabriel", "Gonzalo", "Guillermo", "Hugo", "Ignacio", "Iván", "Javier", "Jesús", "Jorge", 
            "José", "Juan", "Luis", "Manuel", "Marcos", "Mario", "Miguel", "Nicolás", "Óscar", "Pablo", 
            "Pedro", "Rafael", "Ramón", "Ricardo", "Roberto", "Rodrigo", "Rubén", "Salvador", "Santiago", "Sergio", 
            "Tomás", "Víctor", "Adrián", "Álvaro", "Andrés", "Ángel", "Arturo", "Borja", "Bruno", "César", 
            "Cristian", "Darío", "Emilio", "Esteban", "Fabián", "Félix", "Gregorio", "Héctor", "Ismael", "Jaime", 
            "Kevin", "Leonardo", "Lorenzo", "Martín", "Mateo", "Néstor", "Omar", "Patricio", "Raúl", "Samuel", 
            "Simón", "Teodoro", "Ulises", "Valentín", "Xavier", "Aarón", "Abel", "Agustín", "Aitor", "Alberto", 
            "Alejo", "Alfonso", "Alonso", "Amador", "Amancio", "Amando", "Aníbal", "Antolín", "Ariel", "Armando", 
            "Arsenio", "Asier", "Augusto", "Aureliano", "Balthasar", "Bartolomé", "Basilio", "Bautista", "Benigno", "Benito", 
            "Bernabé", "Bernardo", "Blas", "Brais", "Camilo", "Carmelo", "Casimiro", "Cayetano", "Celestino", "Celso", 
            "Clemente", "Conrado", "Constantino", "Cosme", "Cruz", "Custodio", "Damián", "Demetrio", "Desiderio", "Domingo", 
            "Donato", "Edgar", "Edmundo", "Efrén", "Elías", "Eliseo", "Eloy", "Emanuel", "Emiliano", "Erasmo", 
            "Ernesto", "Eugenio", "Eusebio", "Evaristo", "Ezequiel", "Fabián", "Faustino", "Fausto", "Federico", "Feliciano", 
            "Fermín", "Fidel", "Florentino", "Florencio", "Gael", "Gaspar", "Geraldo", "Germán", "Gervasio", "Gil", 
            "Gilberto", "Gorka", "Graciano", "Gumersindo", "Gustavo", "Hernán", "Hilario", "Hipólito", "Honorio", "Horacio", 
            "Iago", "Iker", "Ildefonso", "Imanol", "Íñigo", "Isaac", "Isidoro", "Isidro", "Jacinto", "Jacobo", 
            "Jairo", "Jerónimo", "Joaquín", "Jon", "Jonás", "Joseba", "Josué", "Juanjo", "Julián", "Julio", 
            "Justo", "Lander", "Lázaro", "Leandro", "León", "Leoncio", "Lino", "Lisandro", "Lope", "Lucas", 
            "Luciano", "Macario", "Marc", "Marcelino", "Marcelo", "Marcial", "Mariano", "Marino", "Mateus", "Matías", 
            "Mauricio", "Maximiliano", "Máximo", "Melchor", "Mikel", "Modesto", "Moisés", "Narciso", "Natalio", "Noé", 
            "Norberto", "Octavio", "Odón", "Olegario", "Onofre", "Oriol", "Orlando", "Osvaldo", "Ovidio", "Pascual", 
            "Pastor", "Pau", "Paulino", "Pelayo", "Plácido", "Pol", "Poncio", "Porfirio", "Prudencio", "Quintín", 
            "Raimundo", "Ramiro", "Raymundo", "Reinaldo", "Remigio", "Renato", "Reyes", "Reynaldo", "Rigoberto", "Rogelio", 
            "Rolando", "Román", "Romeo", "Roque", "Rosendo", "Rufino", "Ruperto", "Sabino", "Salomón", "Sancho", 
            "Saturnino", "Sebastián", "Secundino", "Severino", "Severo", "Silvano", "Silvestre", "Tadeo", "Telesforo", "Teobaldo", 
            "Tiburcio", "Tito", "Tobías", "Tristán", "Unai", "Urbano", "Valeriano", "Valero", "Vicente", "Virgilio"
        };

        private static string[] nombresFemeninos = new string[]
        {
            "Ana", "Beatriz", "Carmen", "Dolores", "Elena", "Francisca", "Gloria", "Helena", "Isabel", "Julia", 
            "Laura", "María", "Natalia", "Olga", "Patricia", "Rosa", "Silvia", "Teresa", "Úrsula", "Valentina", 
            "Adriana", "Alicia", "Andrea", "Ángela", "Aurora", "Blanca", "Carlota", "Carolina", "Cristina", "Diana", 
            "Elisa", "Elvira", "Esther", "Eva", "Fátima", "Gabriela", "Inés", "Irene", "Lorena", "Lucía", 
            "Luisa", "Marta", "Mercedes", "Miriam", "Montserrat", "Nerea", "Nuria", "Paula", "Pilar", "Raquel", 
            "Adela", "Adelaida", "Agustina", "Aída", "Ainoa", "Alba", "Alejandra", "Alfonsa", "Almudena", "Amalia", 
            "Amanda", "Amelia", "Amparo", "Anaís", "Anastasia", "Ángeles", "Angélica", "Anita", "Antonia", "Ariadna", 
            "Asunción", "Aurelia", "Bárbara", "Begoña", "Belén", "Benita", "Berta", "Celia", "Clara", "Claudia", 
            "Clementina", "Concepción", "Consuelo", "Coral", "Covadonga", "Cruz", "Daniela", "Darla", "Débora", "Delfina", 
            "Desirée", "Dora", "Dorotea", "Edurne", "Electra", "Eliana", "Elodia", "Eloísa", "Elsa", "Emanuela", 
            "Emilia", "Encarnación", "Enriqueta", "Erica", "Ester", "Estela", "Estefanía", "Eugenia", "Eulalia", "Eusebia", 
            "Fabiola", "Feliciana", "Felisa", "Fernanda", "Fidelia", "Flavia", "Flora", "Florencia", "Frida", "Gala", 
            "Gema", "Genoveva", "Georgina", "Gisela", "Gracia", "Graciela", "Gregoria", "Guadalupe", "Guillermina", "Herminia", 
            "Hilda", "Hortensia", "Ignacia", "Imaculada", "Inmaculada", "Irma", "Isaura", "Isidra", "Jacinta", "Jana", 
            "Javiera", "Jerónima", "Jesusa", "Jimena", "Joaquina", "Josefa", "Josefina", "Juana", "Juanita", "Judith", 
            "Juliana", "Julieta", "Justa", "Justina", "Lara", "Leandra", "Leonor", "Leticia", "Lidia", "Lilia", 
            "Liliana", "Lina", "Lola", "Loreta", "Lourdes", "Luciana", "Lucila", "Lucrecia", "Macarena", "Magdalena", 
            "Maite", "Malena", "Margarita", "Mariana", "Maribel", "Maricarmen", "Marisol", "Marta", "Martina", "Matilde", 
            "Maura", "Máxima", "Maya", "Melania", "Melisa", "Micaela", "Milagros", "Mireia", "Mireya", "Modesta", 
            "Mónica", "Nadia", "Narcisa", "Natividad", "Nayara", "Nazaret", "Nieves", "Noelia", "Noemí", "Ofelia", 
            "Olimpia", "Olivia", "Ondina", "Ovidia", "Pacífica", "Palmira", "Paloma", "Paola", "Pascuala", "Pastora", 
            "Paz", "Penélope", "Petra", "Piedad", "Priscila", "Purificación", "Rafaela", "Ramona", "Rebeca", "Regina", 
            "Remedios", "Renata", "Reyes", "Ricarda", "Rita", "Roberta", "Rocío", "Romina", "Rosalía", "Rosario", 
            "Rosita", "Roxana", "Rufina", "Sabina", "Salomé", "Samanta", "Sandra", "Sara", "Sebastiana", "Selena", 
            "Serafina", "Severina", "Socorro", "Sofía", "Soledad", "Sonia", "Susana", "Tamara", "Tania", "Tatiana", 
            "Teodora", "Tomasa", "Trinidad", "Valeria", "Vanesa", "Vera", "Verónica", "Vicenta", "Victoria", "Virginia"
        };

        private static string[] apellidos = new string[]
        {
            "García", "Martínez", "López", "Sánchez", "González", "Rodríguez", "Fernández", "Pérez", "Gómez", "Martín", 
            "Jiménez", "Ruiz", "Hernández", "Díaz", "Moreno", "Álvarez", "Muñoz", "Romero", "Alonso", "Gutiérrez", 
            "Navarro", "Torres", "Domínguez", "Vázquez", "Ramos", "Gil", "Ramírez", "Serrano", "Blanco", "Molina", 
            "Morales", "Suárez", "Ortega", "Delgado", "Castro", "Ortiz", "Rubio", "Marín", "Sanz", "Iglesias", 
            "Medina", "Garrido", "Cortés", "Castillo", "Santos", "Lozano", "Guerrero", "Cano", "Prieto", "Méndez", 
            "Cruz", "Calvo", "Gallego", "Vidal", "León", "Márquez", "Herrera", "Peña", "Flores", "Cabrera", 
            "Campos", "Vega", "Fuentes", "Carrasco", "Diez", "Caballero", "Reyes", "Nieto", "Aguilar", "Pascual", 
            "Santana", "Herrero", "Lorenzo", "Hidalgo", "Montero", "Ibáñez", "Giménez", "Ferrer", "Durán", "Vicente", 
            "Benítez", "Mora", "Arias", "Santiago", "Carmona", "Crespo", "Pastor", "Soto", "Saez", "Velasco", 
            "Parra", "Moya", "Soler", "Bravo", "Gallardo", "Esteban", "Rojas", "Pardo", "Merino", "Franco", 
            "Espinosa", "Lara", "Izquierdo", "Rivas", "Silva", "Rivera", "Casado", "Arroyo", "Redondo", "Camacho", 
            "Vera", "Otero", "Luque", "Galan", "Montes", "Ríos", "Sierra", "Segura", "Carrillo", "Marcos", 
            "Martí", "Soriano", "Mendoza", "Bernal", "Robles", "Vila", "Valero", "Palacios", "Exposito", "Benito", 
            "Andrés", "Macías", "Pereira", "Guerra", "Heredia", "Bueno", "Roldán", "Mateo", "Villar", "Contreras", 
            "Miranda", "Guillén", "Escobar", "Mateos", "Menéndez", "Aguilera", "Casas", "Aparicio", "Rivero", "Estévez", 
            "Beltrán", "Gracia", "Padilla", "Caldera", "Rico", "Abad", "Gálvez", "Salas", "Conde", "Jurado", 
            "Quintana", "Plaza", "Acosta", "Pino", "Aranda", "Blázquez", "Bermúdez", "Roca", "Guzmán", "Salazar", 
            "Villanueva", "Costa", "Santamaría", "Miguel", "Pacheco", "Cervantes", "Escudero", "Ponce", "Salgado", "Varela", 
            "Puebla", "Carmona", "Rueda", "Castellanos", "Pinto", "Collado", "Cuesta", "Sancho", "Alba", "Godoy", 
            "Tomás", "Linares", "Cordero", "Cuenca", "Mesa", "Saavedra", "Coronado", "Lorente", "Salinas", "Valle", 
            "Mena", "Murillo", "Cobo", "Bermejo", "Zaragoza", "Vaquero", "Agudo", "Alarcón", "Tena", "Domingo", 
            "Aragon", "Gimeno", "Gómez", "Solís", "Ferrando", "Lucas", "Llamas", "Velasco", "Ayala", "Cáceres", 
            "Chaves", "Maldonado", "Manso", "Reig", "Lobo", "Oliva", "Asensio", "Velasquez", "Morillo", "Díaz", 
            "Carretero", "Román", "Olivares", "Peralta", "Villegas", "Luna", "Manzano", "Montoya", "Alcalá", "Cuevas", 
            "Soria", "Del Río", "Marco", "Vergara", "Prado", "Alfonso", "Mínguez", "Paz", "Ochoa", "Vargas", 
            "Maldonado", "Sampedro", "Correa", "Barrios", "Valdés", "Haro", "Palma", "Pizarro", "Navas", "Guevara", 
            "Noguera", "Barros", "Valiente", "Vivas", "Arribas", "Baños", "Siles", "Avilés", "Gámez", "Sevilla"
        };

        private static string[] calles = new string[]
        {
            "Calle de Alcalá", "Paseo de la Castellana", "Gran Vía", "Calle Princesa", "Calle de Atocha", 
            "Calle Mayor", "Calle de Toledo", "Avenida de América", "Paseo del Prado", "Calle Serrano"
        };

        public static SpanishIdentity GenerateRandomIdentity(bool isMale)
        {
            SpanishIdentity id = new SpanishIdentity();
            id.Nombre = isMale ? nombresMasculinos[random.Next(nombresMasculinos.Length)] : nombresFemeninos[random.Next(nombresFemeninos.Length)];
            id.PrimerApellido = apellidos[random.Next(apellidos.Length)];
            id.SegundoApellido = apellidos[random.Next(apellidos.Length)];
            id.NombreCompleto = string.Format("{0} {1} {2}", id.Nombre, id.PrimerApellido, id.SegundoApellido);
            id.DniNumero = DniModulo23.GenerarDniAleatorio();
            
            int age = random.Next(18, 85);
            id.FechaNacimiento = DateTime.Now.AddYears(-age).AddDays(-random.Next(0, 365));
            
            string calle = calles[random.Next(calles.Length)];
            int numero = random.Next(1, 150);
            id.Direccion = string.Format("{0}, {1}, Madrid", calle, numero);

            id.TieneAntecedentes = random.Next(100) < 15; // Probabilidad default
            id.PuntosDGT = random.Next(0, 16);
            
            return id;
        }
    }
}
