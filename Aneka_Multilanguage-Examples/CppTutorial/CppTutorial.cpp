#include "stdafx.h"
#include <iostream>
#include <string>

using namespace System;
using namespace std;
using namespace Aneka;
using namespace Aneka::Threading;
using namespace Aneka::Entity;

[Serializable]
ref class HelloWorld
{
	public:
	int result;
	
	HelloWorld() {}

	void PrintHello()
	{
		result = 1;
	}
};

int main()
{
	AnekaApplication<AnekaThread^, ThreadManager^>^ app;
	try {
		Logger::Start();
		Aneka::Entity::Configuration^ conf = Aneka::Entity::Configuration::GetConfiguration("C:/Aneka/conf.xml");
		app = gcnew AnekaApplication<AnekaThread^, ThreadManager^>(conf);
		HelloWorld^ hw = gcnew HelloWorld();
		AnekaThread^ th = gcnew AnekaThread(hw->PrintHello(), app);
		th->Start();
		th->Join();
		hw = (HelloWorld^)th->Target;
		std::cout << "Value: " << hw->result;
	}
	catch(int e){
		;
	}
	finally{
		app->StopExecution();
		Logger::Stop();
	}


    return 0;
}
