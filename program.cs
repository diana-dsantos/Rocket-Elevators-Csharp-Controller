using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

static class Globals
{
    public static int columnId = 1;
    public static int elevatorID = 1;
    public static int floorRequestButtonID = 1;
    public static int callButtonID = 1;
}

//INIT floor

// ------------------- Class Battery ---------------------------
public class Battery 
{
    public int ID;
    public int amountOfColumns;
    public string status;
    public int amountOfFloors;
    public int amountOfBasements;
    public List<Column> columnsList = new List<Column>();  //List of Column objects
    public List<FloorRequestButton> floorRequestButtonsList = new List<FloorRequestButton>();  //List of CallButton objects - Moderne seulement 


   public Battery(int id, int amountOfColumns,string status, int amountOfFloors, int amountOfBasements, int amountOfElevatorPerColumn)
   {
       this.ID = id;
       this.amountOfFloors = amountOfFloors;
       this.amountOfColumns = amountOfColumns;
       this.status = status;
       this.amountOfColumns = amountOfColumns;
       this.amountOfBasements = amountOfBasements;

       if (amountOfBasements > 0 )
       {
           this.createBasementFloorRequestButtons(amountOfBasements);
           this.createBasementColumn(amountOfBasements, amountOfElevatorPerColumn);
           amountOfColumns--;
       }

       this.createFloorRequestButtons(amountOfFloors);
       this.createColumns(amountOfColumns, amountOfFloors, amountOfElevatorPerColumn);
   }

 // mandatory functions - Moderne seulement : assignElevator(requestedFloor, direction) and findBestColumn(requestedFloor).
//---------------------------------Methods--------------------------------------------//
    public void createBasementFloorRequestButtons(int amountOfBasements)
    {
        int floor = -1;

        FloorRequestButton floorRequestButton = new FloorRequestButton(Globals.floorRequestButtonID, "off", 1, "up");
        this.floorRequestButtonsList.Add(floorRequestButton);

        for(int x=0; x<amountOfBasements; x++) {
            floorRequestButton = new FloorRequestButton(Globals.floorRequestButtonID, "off", floor, "up");
            this.floorRequestButtonsList.Add(floorRequestButton);
            floor--;
            Globals.floorRequestButtonID++;
        }
    }

    public void createBasementColumn(int amountOfBasements, int amountOfElevatorPerColumn)
    {
        List<int> servedFloorsList = new List<int>();
        int floor = -1;

        servedFloorsList.Add(1);
        for(int x=0; x<amountOfBasements; x++) {
            servedFloorsList.Add(floor);
            floor--;
        }

        Column column = new Column(Globals.columnId, "A", "online", amountOfBasements, amountOfElevatorPerColumn, servedFloorsList, true); 
        columnsList.Add(column);
        Globals.columnId++;
    }

    public void createFloorRequestButtons(int amountOfFloors)
    {
        int floor = 1;
        for(int x=1; x<=amountOfFloors; x++) {
            FloorRequestButton floorRequestButton = new FloorRequestButton(Globals.floorRequestButtonID, "off", floor, "up");
            floorRequestButtonsList.Add(floorRequestButton);
            floor++;
            Globals.floorRequestButtonID++;
        }
    }

    public void createColumns(int amountOfColumns, int amountOfFloors, int amountOfElevatorPerColumn)
    {   
        int amountOfFloorsPerColumn = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(amountOfFloors / amountOfColumns)));
        int floor = 1;
        List<string> columnsName = new List<string>(new string[] { "B", "C", "D" });

        for(int x=0; x<amountOfColumns; x++) {
            List<int> servedFloorsList = new List<int>();
            if(x>0){
                servedFloorsList.Add(1);        
            }
            for(int y=1; y<=amountOfFloorsPerColumn; y++) {            
                if(floor <= amountOfFloors) {
                    servedFloorsList.Add(floor);
                    floor++;
                }
            }
            Column column = new Column(Globals.columnId, columnsName.ElementAt(x) ,"online", amountOfBasements, amountOfElevatorPerColumn, servedFloorsList, false); 
            columnsList.Add(column);
            Globals.columnId++;
        }
    }

    //Simulate when a user press a button at the lobby
    public void assignElevator(int currentFloor, int requestedFloor, string direction)
    {
            Thread.Sleep(200);
            var column = this.findBestColumn(currentFloor, requestedFloor);
            
            // The floor is always 1 because that request is always made from the lobby.
            var elevator = column.findElevator(currentFloor, direction);
            
            elevator.floorRequestList.Add(requestedFloor);

            elevator.sortFloorList();
            elevator.move();
            elevator.operateDoors();
    }

    public Column findBestColumn(int currentFloor, int requestedFloor) //return a Column object
    {
        foreach (Column column in this.columnsList)
        {
            if(column.servedFloors.Contains(currentFloor) && column.servedFloors.Contains(requestedFloor))
            {
               return column;
            }
        }
        //TODO remover
        return this.columnsList[0];
    }    
}    

// ------------------- Class Column ---------------------------
public class Column
{
    public int ID;
    public string name;
    public string status;     
    public int amountOfElevators;  //(List of floors)
    public List<int> servedFloors; // to describe which floors are served by each column
    public List<Elevator> elevatorsList = new List<Elevator>(); //Liste d’objets Elevator
    public List<CallButton> callButtonsList = new List<CallButton>(); //Liste d’objets CallButton Classique seulement
    
    public Column(int id, string name, string status, int amountOfFloors, int amountOfElevators, List<int> servedFloors, bool isBasement) 
    {
        this.ID = id;
        this.name = name;
        this.status = status;
        this.amountOfElevators = amountOfElevators;
        this.servedFloors = servedFloors;

        this.createElevators(amountOfFloors, amountOfElevators);
        this.createCallButtons(amountOfFloors, isBasement);
    }

    public void createElevators(int amountOfFloors, int amountOfElevators)
    {
        for(int x=0; x<amountOfElevators; x++) 
        {
            Elevator elevator = new Elevator(Globals.elevatorID, this.name+(x+1) ,"idle", amountOfFloors, 1);
            elevatorsList.Add(elevator);
            Globals.elevatorID++;
        }
    }

    //TODO Incluir em um for somente
    public void createCallButtons(int amountOfFloors, bool isBasement)
    {   
        int buttonFloor =0;
        if(isBasement) 
        {   
            buttonFloor = -1;
            for(int x=0; x<amountOfElevators; x++) 
            {
                CallButton callButton = new CallButton(Globals.elevatorID, "idle", amountOfFloors, null);
                this.callButtonsList.Add(callButton);
                buttonFloor--;              
                Globals.elevatorID++;  
            }
        } 
        else 
        { 
            buttonFloor = 1;
            for(int x=0; x<amountOfElevators; x++) 
            {
                CallButton callButton = new CallButton(Globals.callButtonID, "off", buttonFloor, "up");
                this.callButtonsList.Add(callButton);
                buttonFloor++;              
                Globals.callButtonID++;  
            }
        }        
    }

    //TODO Alterar metodo
    public void requestElevator(int userPosition, string direction)
    {
       Elevator elevator = findElevator(userPosition, direction);
       elevator.floorRequestList.Add(elevator.currentFloor);

        elevator.sortFloorList();
        elevator.move();
        elevator.operateDoors();
    }
    
    /*
    // mandatory functions: requestElevator (requestedFloor, direction).
    public Elevator requestElevator(int requestedFloor, String direction, int userCurrentFloor)
    {
        // if user is not at floor 1 call columnToFindUser_CurrentFloor
        if (userCurrentFloor != 1)
        {
            Column columnFinded = this.columnToFindUser_CurrentFloor(userCurrentFloor);
            // calling function ElevatorInTheChosenColumn
            Elevator elevatorFinded = columnFinded.ElevatorInTheChosenColumn(columnFinded, requestedFloor, direction, userCurrentFloor);
            Console.WriteLine("Elevator choosen is : " + elevatorFinded.id);
            return elevatorFinded;
        }// else call columnToFind_requestedFloor
        else
        {
            Column columnFinded = this.columnToFind_requestedFloor(requestedFloor);
            // calling function ElevatorInTheChosenColumn
            Elevator elevatorFinded = columnFinded.ElevatorInTheChosenColumn(columnFinded, requestedFloor, direction, userCurrentFloor);
            Console.WriteLine("Elevator choosen is : " + elevatorFinded.id);
            return elevatorFinded;
        }
    }
    */


    public Elevator findElevator(int requestedFloor, string requestedDirection)
    {   
        BestElevatorInformations bestElevatorInformations = new BestElevatorInformations(null, 6, 10000000);
        
        if (requestedFloor ==  1) {
            foreach (Elevator elevator in elevatorsList) 
            {
                //The elevator is at the lobby and already has some requests. It is about to leave but has not yet departed
                if (1 == elevator.currentFloor && elevator.status == "stopped")
                {
                    bestElevatorInformations = checkifElevatorIsBetter(1, elevator, bestElevatorInformations, requestedFloor);
                }
                //The elevator is at the lobby and has no requests
                else if (1 == elevator.currentFloor && elevator.status == "idle")
                {
                    bestElevatorInformations = checkifElevatorIsBetter(2, elevator, bestElevatorInformations, requestedFloor);
                //The elevator is lower than me and is coming up. It means that I am requesting an elevator to go to a basement, and the elevator is on it's way to me.
                }
                else if (1 > elevator.currentFloor && elevator.direction == "up")
                {
                    bestElevatorInformations = checkifElevatorIsBetter(3, elevator, bestElevatorInformations, requestedFloor);
                }
                //The elevator is above me and is coming down. It means that I'm requesting an elevator to go to a floor, and the elevator is on it's way to me
                else if (1 < elevator.currentFloor && elevator.direction == "down")
                {
                    bestElevatorInformations = checkifElevatorIsBetter(3, elevator, bestElevatorInformations, requestedFloor);
                }//The elevator is not at the first floor, but doesn't have any request
                else if (elevator.status == "idle")
                {
                    bestElevatorInformations = checkifElevatorIsBetter(4, elevator, bestElevatorInformations, requestedFloor);
                } //The elevator is not available, but still could take the call if nothing better is found
                else 
                {
                    bestElevatorInformations = checkifElevatorIsBetter(5, elevator, bestElevatorInformations, requestedFloor);
                }
            }
        }
        else {
             foreach (Elevator elevator in elevatorsList) {
                //The elevator is at the same level as me, and is about to depart to the first floor
                if (requestedFloor ==  elevator.currentFloor && elevator.status == "stopped" && requestedDirection == elevator.direction) 
                {
                    bestElevatorInformations = checkifElevatorIsBetter(1, elevator, bestElevatorInformations, requestedFloor);
                }
                //The elevator is lower than me and is going up. I'm on a basement, and the elevator can pick me up on it's way
                else if (requestedFloor > elevator.currentFloor && elevator.direction == "up" && requestedDirection == "up" ) 
                {
                    bestElevatorInformations = checkifElevatorIsBetter(2, elevator, bestElevatorInformations, requestedFloor);
                } //The elevator is higher than me and is going down. I'm on a floor, and the elevator can pick me up on it's way
                else if (requestedFloor < elevator.currentFloor && elevator.direction == "down" && requestedDirection == "down")
                {
                    bestElevatorInformations = checkifElevatorIsBetter(2, elevator, bestElevatorInformations, requestedFloor);
                }//The elevator is idle and has no requests
                else if (elevator.status == "idle") 
                {
                    bestElevatorInformations = checkifElevatorIsBetter(4, elevator, bestElevatorInformations, requestedFloor);
                }//The elevator is not available, but still could take the call if nothing better is found
                else 
                {
                    bestElevatorInformations = checkifElevatorIsBetter(5, elevator, bestElevatorInformations, requestedFloor);
                }
            }
        }
        return bestElevatorInformations.bestElevator;     
    }

    public BestElevatorInformations checkifElevatorIsBetter(int scoreToCheck, Elevator newElevator, BestElevatorInformations bestElevatorInformations, int requestedFloor)
    {   
        int gap = Math.Abs(newElevator.currentFloor - requestedFloor);
        if( scoreToCheck < bestElevatorInformations.bestScore || ( scoreToCheck == bestElevatorInformations.bestScore && bestElevatorInformations.referenceGap > gap ))
        {   
            bestElevatorInformations.bestScore = scoreToCheck;
            bestElevatorInformations.bestElevator = newElevator;
            bestElevatorInformations.referenceGap = gap;
        }
        return bestElevatorInformations;    
    }
}
// ------------------- Class Elevators  ---------------------------
public class Elevator 
{
    public int ID;
    public string name; 
    public string status; 
    public string direction;
    public int currentFloor;   
    public int screenDisplay;
    public int amountOfFloors;
    public Door door;  
    public List<int> floorRequestList = new List<int>();

    public Elevator(int id, string name, string status, int amountOfFloors, int currentFloor)
    {
        this.ID = id;
        this.name = name;
        this.status = status;
        this.amountOfFloors = amountOfFloors;
        this.currentFloor = currentFloor;
        this.door = new Door(this.ID, "Closed");
        this.direction = null;
        this.screenDisplay = currentFloor;
    } 
    //mandatory functions: requestFloor(requestedFloor).-- Classique seulement
    //---------------------------------Methods--------------------------------------------//
    
    public void move()
    {   
        int destination =0;
        while(this.floorRequestList.Any()) 
        {
            destination = floorRequestList.First();
            this.status = "moving";
            while (this.currentFloor != destination) 
            {
                if (this.currentFloor < destination) 
                {
                    this.direction = "up";
                    currentFloor++;
                }
                else {
                    this.direction = "down";
                    currentFloor--;
                }                
                this.screenDisplay = currentFloor;
            }
            this.status = "stopped";
            floorRequestList.RemoveAt(0);
        }        
    }

    public void move(int requestedFloor, string direction)
    {           
        this.floorRequestList.Add(requestedFloor);
        this.move();
    }

    //TODO Implementar esse metodo
    public void sortFloorList()
    {   

        if (this.direction == "down" )
        {
            //SORT THIS requestList ASCENDING
        }
        else 
        {
            //SORT THIS requestList DESCENDING
        }
    }

    public void operateDoors()
    {   
        this.door.status = "open";
        Thread.Sleep(500);
        this.door.status = "closing"; 
        Thread.Sleep(50);
        this.door.status = "closed"; 
    }
}

// ------------------- Class CallButton ---------------------------
public class CallButton
{
    public int ID;
    public string status;
    public int floor;
    public string direction;

    public CallButton (int id,string status,int floor,string direction)
    {
        this.ID = id;
        this.status = status;
        this.floor = floor;
        this.direction = null;
    }
}


// ------------------- Class FloorRequestButton ---------------------------
public class FloorRequestButton
{
    public int ID;
    public string status;
    public int floor;
    public string direction;

    public FloorRequestButton(int id,string status,int floor, string direction)
    {
        this.ID = id;
        this.status = status;
        this.floor = floor;
        this.direction = null;
    }
}


// ------------------- Class CallButton ---------------------------
public class Door
{
    public int ID;
    public string status;

    public Door(int id, string status)
    {
        this.ID = id;
        this.status = status;
    }
}

// ------------------- Class BestElevator ---------------------------
public class BestElevatorInformations
{
    public Elevator bestElevator;
    public int bestScore;
    public int referenceGap;

    public BestElevatorInformations(Elevator bestElevator,int bestScore, int referenceGap)
    {
        this.bestElevator = bestElevator;
        this.bestScore = bestScore;
        this.referenceGap = referenceGap;
    }
}



class Program {
    public static void Main(string[] args) {

        Scenarios.scenario_1();
        Scenarios.scenario_2();
        Scenarios.scenario_3();
        Scenarios.scenario_4();
    }


}

static class Scenarios
{       
        public static void scenario_1()
    {        
        Battery battery = new Battery(1, 4, "online", 60, 6, 5);

        Column columnB = battery.columnsList.ElementAt(1);  

        Elevator elevatorB1 = columnB.elevatorsList.ElementAt(0);
        elevatorB1.currentFloor = 20;        
        elevatorB1.direction = "down";
        elevatorB1.status = "moving";
        elevatorB1.floorRequestList.Add(5);

        Elevator elevatorB2 = columnB.elevatorsList.ElementAt(1);
        elevatorB2.currentFloor = 3;
        elevatorB2.direction = "up";
        elevatorB2.status = "moving";
        elevatorB2.floorRequestList.Add(15);

        Elevator elevatorB3 = columnB.elevatorsList.ElementAt(2);
        elevatorB3.currentFloor = 13;
        elevatorB3.direction = "down";
        elevatorB3.status = "moving";
        elevatorB3.floorRequestList.Add(1);

        Elevator elevatorB4 = columnB.elevatorsList.ElementAt(3);
        elevatorB4.currentFloor = 15;
        elevatorB4.direction = "down";
        elevatorB4.status = "moving";
        elevatorB4.floorRequestList.Add(2);

        Elevator elevatorB5 = columnB.elevatorsList.ElementAt(4);
        elevatorB5.currentFloor = 6;
        elevatorB5.direction = "down";
        elevatorB5.status = "moving";
        elevatorB5.floorRequestList.Add(1);

        battery.assignElevator(1, 20, "up");
    }

 public static void scenario_2()
    {        
        Battery battery = new Battery(1, 4, "online", 60, 6, 5);

        Column columnC = battery.columnsList.ElementAt(3);  

        Elevator elevatorC1 = columnC.elevatorsList.ElementAt(0);
        elevatorC1.currentFloor = 1;        
        elevatorC1.direction = "up";
        elevatorC1.status = "stopped";
        elevatorC1.floorRequestList.Add(21);

        Elevator elevatorC2 = columnC.elevatorsList.ElementAt(1);
        elevatorC2.currentFloor = 23;
        elevatorC2.direction = "up";
        elevatorC2.status = "moving";
        elevatorC2.floorRequestList.Add(28);

        Elevator elevatorC3 = columnC.elevatorsList.ElementAt(2);
        elevatorC3.currentFloor = 33;
        elevatorC3.direction = "down";
        elevatorC3.status = "moving";
        elevatorC3.floorRequestList.Add(1);

        Elevator elevatorC4 = columnC.elevatorsList.ElementAt(3);
        elevatorC4.currentFloor = 40;
        elevatorC4.direction = "down";
        elevatorC4.status = "moving";
        elevatorC4.floorRequestList.Add(24);

        Elevator elevatorC5 = columnC.elevatorsList.ElementAt(4);
        elevatorC5.currentFloor = 39;
        elevatorC5.direction = "down";
        elevatorC5.status = "moving";
        elevatorC5.floorRequestList.Add(1);

        battery.assignElevator(1, 36, "up");
    }
    
    public static void scenario_3()
    {        
        Battery battery = new Battery(1, 4, "online", 60, 6, 5);

        Column columnD = battery.columnsList.ElementAt(3);  

        Elevator elevatorD1 = columnD.elevatorsList.ElementAt(0);
        elevatorD1.currentFloor = 58;        
        elevatorD1.direction = "down";
        elevatorD1.status = "moving";
        elevatorD1.floorRequestList.Add(1);

        Elevator elevatorD2 = columnD.elevatorsList.ElementAt(1);
        elevatorD2.currentFloor = 50;
        elevatorD2.direction = "up";
        elevatorD2.status = "moving";
        elevatorD2.floorRequestList.Add(60);

        Elevator elevatorD3 = columnD.elevatorsList.ElementAt(2);
        elevatorD3.currentFloor = 46;
        elevatorD3.direction = "up";
        elevatorD3.status = "moving";
        elevatorD3.floorRequestList.Add(58);

        Elevator elevatorD4 = columnD.elevatorsList.ElementAt(3);
        elevatorD4.currentFloor = 1;
        elevatorD4.direction = "up";
        elevatorD4.status = "moving";
        elevatorD4.floorRequestList.Add(54);

        Elevator elevatorD5 = columnD.elevatorsList.ElementAt(4);
        elevatorD5.currentFloor = 60;
        elevatorD5.direction = "down";
        elevatorD5.status = "moving";
        elevatorD5.floorRequestList.Add(1);

        battery.assignElevator(54, 1, "down");
    }

    public static void scenario_4()
    {        
        Battery battery = new Battery(1, 4, "online", 60, 6, 5);

        Column columnA = battery.columnsList.ElementAt(0);  

        Elevator elevatorA1 = columnA.elevatorsList.ElementAt(0);
        elevatorA1.currentFloor = -4;        
        elevatorA1.direction = null;
        elevatorA1.status = "idle";

        Elevator elevatorA2 = columnA.elevatorsList.ElementAt(1);
        elevatorA2.currentFloor = 1;
        elevatorA2.direction = null;
        elevatorA2.status = "idle";       

        Elevator elevatorA3 = columnA.elevatorsList.ElementAt(2);
        elevatorA3.currentFloor = -3;
        elevatorA3.direction = "down";
        elevatorA3.status = "moving";
        elevatorA3.floorRequestList.Add(-5);

        Elevator elevatorA4 = columnA.elevatorsList.ElementAt(3);
        elevatorA4.currentFloor = -6;
        elevatorA4.direction = "up";
        elevatorA4.status = "moving";
        elevatorA4.floorRequestList.Add(1);

        Elevator elevatorA5 = columnA.elevatorsList.ElementAt(4);
        elevatorA5.currentFloor = -1;
        elevatorA5.direction = "down";
        elevatorA5.status = "moving";
        elevatorA5.floorRequestList.Add(-6);

        battery.assignElevator(-3, 1, "up");
    }
}

//==================================Scenario 1=================================================


// SET battery TO NEW Battery WITH 1 AND 4 AND online AND 60 AND 6 AND 5 '//id, amountOfColumns, status, amountOfFloors, amountOfBasements, amountOfElevatorsPerColumn
// SET column TO second column OF battery columnsList

// bate

// '//We put everything in place for the scenario
// SET floor OF first elevator OF column elevatorsList TO 20
// SET direction OF first elevator OF column elevatorsList TO Down
// ADD 5 TO requestList OF first elevator OF column elevatorsList

// SET floor OF second elevator OF column elevatorsList TO 3
// SET direction OF second elevator OF column elevatorsList TO Up

// ADD 15 TO requestList OF second elevator OF column elevatorsList columnsList

// SET floor OF third elevator OF column elevatorsList TO 13
// SET direction OF third elevator OF column elevatorsList TO Down
// ADD 1 TO requestList OF third elevator OF column elevatorsList

// SET floor OF fourth elevator OF column elevatorsList TO 15
// SET direction OF fourth elevator OF column elevatorsList TO Down
// ADD 2 TO requestList OF fourth elevator OF column elevatorsList

// SET floor OF fifth elevator OF column elevatorsList TO 6
// SET direction OF fifth elevator OF column elevatorsList TO Down
// ADD 1 TO requestList OF fifth elevator OF column elevatorsList

// '//We make the request
// CALL battery assignElevator WITH 20 AND Up
//==================================End Scenario 1=============================================
//Scenario1();
//Scenario2();
//Scenario3();
//Scenario4();
