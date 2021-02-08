// ------------------- Class Battery ---------------------------
class Battery {
    public int ID;
    public int amountOfColumns;
    public int status;
    public int amountOfFloors;
    public int amountOfBasements;
    public callButtonsList;//(List of CallButton objects) //Moderne seulement 
    public List<Column> listColumnInBattery;//List of Column objects
    public Battery(int id, int nbOfColumnInBattery, int nbOfBasement, List<Column> listColumnInBattery)
        {
            this.id = id;
            this.nbOfColumnInBattery = nbOfColumnInBattery;
            this.nbOfBasement = nbOfBasement;
            this.listColumnInBattery = listColumnInBattery;
        } 
}

// mandatory functions - Moderne seulement : assignElevator(requestedFloor, direction) and findBestColumn(requestedFloor). Devrait retourner un objet Column


// ------------------- Class Column ---------------------------
class Column(id,status, amountOfFloors, amountOfElevators, servedFloors, isBasement) {

    public int ID;
    public int status; 
    public int servedFloors; // to describe which floors are served by each column
    public int amountOfElevators;  //(List of floors)
    public elevatorsList; //(Liste d’objets Elevator)
    public callButtonsList; //(Liste d’objets CallButton) Classique seulement
    public floorRequestButtonsList; //(Liste d’objets FloorRequestButton) Moderne seulement

    }

   // mandatory functions: requestElevator (requestedFloor, direction).

// ------------------- Class Elevators  ---------------------------
class Elevator(id, status, amountOfFloors, currentFloor) {
    public int ID;
    public int status;
    public int currentFloor;
    public int direction;
    public int door; //Door object
    public floorRequestButtonsList; //Liste d’objets FloorRequestButton  -  Classique seulement
    public floorRequestList:

    //mandatory functions: requestFloor(requestedFloor).-- Classique seulement

}

// ------------------- Class CallButton ---------------------------
class CallButton(id, status, floor, direction){
    public int ID;
    public int status;
    public int floor;
    public int direction
}

// ------------------- Class FloorRequestButton ---------------------------
class FloorRequestButton(id, status, floor){
    public int ID;
    public int status;
    public int floor;
}

// ------------------- Class CallButton ---------------------------
class Door(id, status){
    public int ID;
    public int status;
}