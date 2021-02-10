using System;
using System.Collections.Generic;
using System.Threading;

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
       }

       this.createFloorRequestButtons(amountOfFloors);
       this.createColumns(amountOfColumns, amountOfFloors, amountOfElevatorPerColumn);
   }

 // mandatory functions - Moderne seulement : assignElevator(requestedFloor, direction) and findBestColumn(requestedFloor).
//---------------------------------Methods--------------------------------------------//
    public void createBasementFloorRequestButtons(int amountOfBasements)
    {
        int floor = -1;
        for(int x=0; x<amountOfBasements; x++) {
            FloorRequestButton floorRequestButton = new FloorRequestButton(Globals.floorRequestButtonID, "off", floor, "up");
            this.floorRequestButtonsList.Add(floorRequestButton);
            floor--;
            Globals.floorRequestButtonID++;
        }
    }

    public void createBasementColumn(int amountOfBasements, int amountOfElevatorPerColumn)
    {
        List<int> servedFloorsList = new List<int>();
        int floor = -1;

        for(int x=0; x<amountOfBasements; x++) {
            servedFloorsList.Add(floor);
            floor--;
        }

        Column column = new Column(Globals.columnId, "online", amountOfBasements, amountOfElevatorPerColumn, servedFloorsList, true); 
        columnsList.Add(column);
        Globals.columnId++;
    }

    public void createFloorRequestButtons(int amountOfFloors)
    {
        int floor = 1;
        for(int x=0; x<amountOfFloors; x++) {
            FloorRequestButton floorRequestButton = new FloorRequestButton(Globals.floorRequestButtonID, "off", floor, "up");
            floorRequestButtonsList.Add(floorRequestButton);
            floor++;
            Globals.floorRequestButtonID++;
        }
    }

    public void createColumns(int amountOfColumns, int amountOfFloors, int amountOfElevatorPerColumn)
    {
        int amountOfFloorsPerColumn = Math.Ceiling(amountOfFloors / amountOfColumns);
        int floor = 1;

        for(int x=0; x<amountOfColumns; x++) {
            List<int> servedFloorsList = new List<int>();
            for(int y=0; x<amountOfFloorsPerColumn; y++) {
                if(floor <= amountOfFloors) {
                    servedFloors.Add(floor);
                    floor++;
                }
            }
        }

        Column column = new Column(Globals.columnId, "online", amountOfBasements, amountOfElevatorPerColumn, servedFloorsList, true); 
        columnsList.Add(column);
        Globals.columnId++;
    }

    //Simulate when a user press a button at the lobby
    public Elevator assignElevator(int requestedFloor, string direction)
    {
            Thread.Sleep(200);
            var column = battery.findBestColumn(requestedFloor);
            
            //TODO Talvez alterar requestedFloor to 1
            var elevator = column.findElevator(requestedFloor, direction);

            elevator.floorRequestList.Add(requestedFloor);

            elevator.sortFloorList();
            elevator.move();
            elevator.operateDoors();
    }

    // public void operateDoors()
    // {            
    //         if (elevator.elevatorFloor > floorNumber)
    //         {
    //             elevator.addtoFloorQueue(floorNumber, column.columnNumber);
    //             elevator.addtoFloorQueue(requestedFloor, column.columnNumber);
    //         }
    //         else if (elevator.elevatorFloor < floorNumber)
    //         {
    //             elevator.addtoFloorQueue(floorNumber, column.columnNumber);
    //             elevator.addtoFloorQueue(requestedFloor, column.columnNumber);
    //         }
    //         return elevator;  
    // }

    public Column findBestColumn(int requestedFloor) //return a Column object
    {
        foreach (Column column in columnList)
        {
            if(column.servedFloors.contains(requestedFloor))
            {
               return column;
            }
        }    
    }    
}    

// ------------------- Class Column ---------------------------
public class Column
{
    public int ID;
    public string status;     
    public int amountOfElevators;  //(List of floors)
    public List<int> servedFloors; // to describe which floors are served by each column
    public List<Elevator> elevatorsList = new List<Elevator>(); //Liste d’objets Elevator
    public List<CallButton> callButtonsList = new List<CallButton>(); //Liste d’objets CallButton Classique seulement
    
    public Column(int id, string status, int amountOfFloors, int amountOfElevators, List<int> servedFloors, int isBasement) 
    {
        this.ID = id;
        this.status = status;
        this.amountOfElevators = amountOfElevators;
        this.serveFloors = servedFloors;

        this.createElevators(amountOfFloors, amountOfElevators);
        this.createCallButtons(amountOfFloors, isBasement);
    }

    public void createElevators(int amountOfFloors, int amountOfElevators)
    {
        for(int x=0; x<amountOfElevators; x++) 
        {
            Elevator elevator = new Elevator(Globals.elevatorID, "idle", amountOfFloors, 1);
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
                CallButton callButton = new CallButton(Globals.elevatorID, "idle", amountOfFloors, 1);
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
       Elevator elevator = findElevator(userPosition, direction)
       elevator.requestList.Add(1);

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


    public Elevator findElevator(int requestedFloor, bool requestedDirection)
    {   
        BestElevatorInformations bestElevatorInformations = new BestElevatorInformations(null, 6, 10000000);
        
        if (bestElevatorInformations.requestFloor ==  1) {
            foreach (Elevator elevator in elevatorsList) 
            {
                //The elevator is at the lobby and already has some requests. It is about to leave but has not yet departed
                if (1 == elevator.currentFloor && elevator.status == "stopped")
                {
                    bestElevatorInformations = checkifElevatorIsBetter(1, bestElevatorInformations.elevator, bestElevatorInformations.bestScore, bestElevatorInformations.referenceGap, bestElevatorInformations.bestElevator);
                }
                //The elevator is at the lobby and has no requests
                else if (1 == elevator.currentFloor && elevator.status == "idle")
                {
                    bestElevatorInformations = checkifElevatorIsBetter(2, bestElevatorInformations.elevator, bestElevatorInformations.bestScore, bestElevatorInformations.referenceGap, bestElevatorInformations.bestElevator);
                //The elevator is lower than me and is coming up. It means that I am requesting an elevator to go to a basement, and the elevator is on it's way to me.
                }
                else if (1 > elevator.currentFloor && elevator.direction == "up")
                {
                    bestElevatorInformations = checkifElevatorIsBetter(3, bestElevatorInformations.elevator, bestElevatorInformations.bestScore, bestElevatorInformations.referenceGap, bestElevatorInformations.bestElevator);
                }
                //The elevator is above me and is coming down. It means that I'm requesting an elevator to go to a floor, and the elevator is on it's way to me
                else if (1 > elevator.currentFloor && elevator.direction == "down")
                {
                    bestElevatorInformations = checkifElevatorIsBetter(3, bestElevatorInformations.elevator, bestElevatorInformations.bestScore, bestElevatorInformations.referenceGap, bestElevatorInformations.bestElevator);
                }//The elevator is not at the first floor, but doesn't have any request
                else if (elevator.status == "idle")
                {
                    bestElevatorInformations = checkifElevatorIsBetter(4, bestElevatorInformations.elevator, bestElevatorInformations.bestScore, bestElevatorInformations.referenceGap, bestElevatorInformations.bestElevator);
                } //The elevator is not available, but still could take the call if nothing better is found
                else 
                {
                    bestElevatorInformations = checkifElevatorIsBetter(5, bestElevatorInformations.elevator, bestElevatorInformations.bestScore, bestElevatorInformations.referenceGap, bestElevatorInformations.bestElevator);
                }
            }
        }
        else {
             foreach (Elevator elevator in elevatorsList) {
                //The elevator is at the same level as me, and is about to depart to the first floor
                if (requetedFloor ==  elevator.currentFloor && elevator.status == "stopped" && requestedDirection == elevator.direction) 
                {
                    bestElevatorInformations = checkifElevatorIsBetter(1, bestElevatorInformations.elevator, bestElevatorInformations.bestScore, bestElevatorInformations.referenceGap, bestElevatorInformations.bestElevator);
                }
                //The elevator is lower than me and is going up. I'm on a basement, and the elevator can pick me up on it's way
                else if (elevator.direction.requestedFloor > elevator.currentFloor && elevator.direction == "up" && requestedDirection == "up" ) 
                {
                    bestElevatorInformations = checkifElevatorIsBetter(2, bestElevatorInformations.elevator, bestElevatorInformations.bestScore, bestElevatorInformations.referenceGap, bestElevatorInformations.bestElevator);
                } //The elevator is higher than me and is going down. I'm on a floor, and the elevator can pick me up on it's way
                else if (requestedFloor < elevator.currentFloor && elevator.direction == "down" && requestedDirection == "down")
                {
                    bestElevatorInformations = checkifElevatorIsBetter(2, bestElevatorInformations.elevator, bestElevatorInformations.bestScore, bestElevatorInformations.referenceGap, bestElevatorInformations.bestElevator);
                }//The elevator is idle and has no requests
                else if (elevator.status == "idle") 
                {
                    bestElevatorInformations = checkifElevatorIsBetter(4, bestElevatorInformations.elevator, bestElevatorInformations.bestScore, bestElevatorInformations.referenceGap, bestElevatorInformations.bestElevator);
                }//The elevator is not available, but still could take the call if nothing better is found
                else 
                {
                    bestElevatorInformations = checkifElevatorIsBetter(5, bestElevatorInformations.elevator, bestElevatorInformations.bestScore, bestElevatorInformations.referenceGap, bestElevatorInformations.bestElevator);
                }
            }
        }
        return bestElevatorInformations.elevator;     
    }

    public void checkifElevatorIsBetter(int scoreToCheck, Elevator newElevator, int bestScore, int referenceGap, BestElevatorInformations bestElevatorInformations, int requestedFloor)
    {   
        int gap = Math.Abs(newElevator.currentFloor - requestedFloor);
        if( scoreToCheck < bestScore || ( scoreToCheck == bestScore && bestElevatorInformations.referenceGap > gap ))
        {   
            bestElevatorInformations.bestScore = scoreToCheck;
            bestElevatorInformations.elevator = newElevator;
            bestElevatorInformations.referenceGap = gap;
        }

        return bestElevatorInformations;
    }
}
// ------------------- Class Elevators  ---------------------------
public class Elevator 
{
    public int ID;
    public string status; 
    public string direction;
    public int currentFloor;   
    public int door; //Door object    
    public List<FloorRequestButton> floorRequestList = new List<FloorRequestButton>();

    public Elevator(int id, string status, int amountOfFloors, int currentFloor)
    {
        this.ID = id;
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
        while(!this.floorRequestList.Any()) 
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
        //IF THIS IS NOT overweight THEN
            this.door.status = "closing";
            // IF no obstruction THEN
            //     SET THIS door status TO closed
            // ELSE
            //     CALL THIS operateDoors
        // ELSE
        //     WHILE THIS IS overweight
        //        Activate overweight alarm
        //     ENDWHILE
        //     CALL THIS operateDoors        
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
    public Elevator elevator;
    public int bestScore;
    public int referenceGap;

    public BestElevatorInformations(Elevator elevator,int bestScore, int referenceGap)
    {
        this.elevator = elevator;
        this.bestScore = bestScore;
        this.referenceGap = referenceGap;
    }
}
