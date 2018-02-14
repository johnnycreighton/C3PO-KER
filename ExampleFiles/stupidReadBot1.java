import java.util.*;
import java.io.*;
class Bot1{
  public static void main(String[] args) {
	 Random rng= new Random();
	StringWriter everything= new StringWriter();
	//stupid pokerBot- call 95%, fold 5%
	
	
	int i=0;
	while(true){
	try(StringWriter a=readFromFile("./Casino/bots/bot1.txt"))
	{
	//if(!a.toString().equals(everything.toString())){
	Thread.sleep(rng.nextInt(15));
	if(!a.toString().equals("c") || !a.toString().equals("f") ){
	if(rng.nextInt(100)<=95){
	printToFile("./Casino/bots/bot1.txt", "c");
	//System.out.println("c");
	}
	else{
	printToFile("./Casino/bots/bot1.txt", "f");
	//System.out.println("f");
	}
	}
	/*
	if (i==2){
		System.out.println("Here is a: ");
		System.out.println(a);
		System.out.println("Here is everything: ");
		System.out.println(everything);
	}
	*/
		else{
	Thread.sleep(rng.nextInt(15));
	}
	a.flush();
	}

	
	catch(IOException e){
	

	}
	
	catch(InterruptedException e){
	

	}
	i++;
 }
}

static StringWriter readFromFile(String path) throws IOException {
	 StringWriter stringWriter = new StringWriter();
    try (BufferedReader br =
                   new BufferedReader(new FileReader(path))) {
		StringBuilder sb = new StringBuilder();
		String line = br.readLine();
		while (line != null) {
        stringWriter.append(line + "\n");
        line = br.readLine();
    }			   
		
        return stringWriter;
    }
}

static void printToFile(String path, String message){
StringWriter stringWriter = new StringWriter();
try( FileWriter fileWriter = new FileWriter(path))
{
		PrintWriter printWriter = new PrintWriter(fileWriter);
		printWriter.print(message);
		 printWriter.close();
}
catch(IOException e){
printToFile(path, message);
}
 }


/*
static void printToFile(String path, String message) throws IOException {
	 StringWriter stringWriter = new StringWriter();
	 FileWriter fileWriter = new FileWriter(fileName);
    
	try ( FileWriter fileWriter = new FileWriter(path) {
		PrintWriter printWriter = new PrintWriter(fileWriter);
		printWriter.print(message);
		 printWriter.close();
    }			   
		

    }*/
}



