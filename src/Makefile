#Make file pro Team.toString projekt

#Názvy složek
#	Název Složky projektu
	main	:= Team.toString
#	Název složky pro výstup profilingu
	m_prf	:= profiling
#	Názvy složek v src
	gui 	:= GUI
	prf 	:= Profiling
	test	:= MathLibTests
#	Cesta k latex dokumentaci
	doc     	:= doc\latex
	doc_file	:= $(doc)\refman.pdf
	doc_name	:= dokumentace.pdf

#Název finálního archívu
archive := xkosik10_xkoren11_xkocic02

#Nastavení pro profiling
#	Označení vstupů pro profiling (vstup$(input_c).txt)
	input_c		 	:= 10 100 1000
#	Možnosti pro profiling
	profile_options := collect --profile cpu-sampling --show-child-io
#	Převod čísle na název cílů
	profiles		:= $(addprefix profile,${input_c})

#Nastavení .NET
#	Verze .NET (první dvě císla)
	dot_v		:= 5.0
#	Název složky do které se generuje výstup překladu v jednotlivých modulech
	dot_f_name	:= net$(dot_v)
#	Cílová platforma
	os			:= win-x64

#Sestavený profiling program
prf_out := $(prf)\bin\Release\$(dot_f_name)\win-x64\publish

#Nastavení publish
publish_opt := --configuration Release -r $(os) -p:Version=$(dot_v) -p:PublishSingleFile=true -p:PublishTrimmed=true --self-contained true

#sestaví hlavní program a profiling program
all:build_gui build_prf

#vytvoří archiv repositáře
pack: pdf clean
	echo "create folders for temp files"
	cd .. && cd .. && mkdir "$(archive)\doc"
	cd .. && cd .. && mkdir "$(archive)\install"
	cd .. && cd .. && mkdir "$(archive)\repo"

	echo "copy required files"
	cd .. && cd .. && xcopy /b /s /y /q ".\$(main)\$(doc_name)"     ".\$(archive)\doc"
	cd .. && cd .. && xcopy /b /s /y /q ".\$(main)\install\*"       ".\$(archive)\install"
	cd .. && cd .. && xcopy /b /s /y /q ".\$(main)\*"               ".\$(archive)\repo"

	echo "create archive"
	cd .. && cd .. && tar -acf $(archive).zip $(archive)

	echo "delete temp files"
	cd .. && cd .. && rd /s /q "$(archive)"

#vyčistí repositář od věcí které jdou vygenerovat
clean: clean_profile
#	smaže všechy vygenerovatelné soubory ve složce src
	echo "delete bin subfolders"
	FOR /d /r . %%d IN (bin) DO @IF EXIST "%%d" rd /s /q "%%d"
	echo "delete obj subfolders"
	FOR /d /r . %%d IN (obj) DO @IF EXIST "%%d" rd /s /q "%%d"
	echo "delete doc folder"
	@IF EXIST "doc" rd /s /q "doc"
	
#	smaže záznamy o testech z hlaví projektové složky
	echo "delete test folders and files"
	cd .. && @IF EXIST "TestResults" rd /s /q "TestResults"

#uklízí profiling output
clean_profile:
	cd .. && cd ./$(m_prf) && @IF EXIST "Profiling.exe"\
		del /f "Profiling.exe" &&\
		del /S *.dll &&\
		del /S *.pdb
	

#spustí testy na matematickou knihovnu
test:
	cd .\$(test) && dotnet test
	
#vytvoří dokumentaci
doc:
	doxygen

#vytvoří pdf soubor dokumentace
pdf: doc
	echo "generates pdf from latex"
	cd doc\latex && make.bat 1>Nul
	echo "renames pdf and moves pdf to correct folder"
	cd .. && copy ".\src\$(doc_file)" ".\$(doc_name)"

#spusí aplikaci
run: build_gui
	cd .\$(gui) && dotnet run
	
#naistaluje potřebné nástoroje pro profiling
dot_trace_install:
	dotnet tool install dotnet-trace --global
	
#spustí profiling pro všechny vstupy
profile: setup_profile
# 	spouští jednotlivé profilingy
	make $(profiles)
	make clean_profile
	
#připaví profiling složku pro profiling
setup_profile:
	cd .. && @IF NOT EXIST .\$(m_prf)\Profiling.exe cd .\src &&\
			make publish_prf &&\
		cd .. &&\
		xcopy /b /s /y /q ".\src\$(prf_out)" ".\$(m_prf)"

#spustí konkrétní profiling
$(profiles): profile%:
#	spustí profiling s argumenty a vygeneruje výstup
#	smaže prázdné error logy (jsou prázdné když nedojde k žádnému selhání)
	cd .. && cd .\$(m_prf) &&\
		dotnet-trace $(profile_options) --show-child-io --output vystup$*.nettrace -- Profiling.exe <vstup$*.txt 1>vystup$*.txt 2>vystup_err$*.txt &&\
		findstr "^" "vystup_err$*.txt" || del "vystup_err$*.txt"
#sestaví aplikaci
build_gui:
	cd .\$(gui) && dotnet build --configuration Release

#sestaví program pro profiling
build_prf:
	cd .\$(prf) && dotnet build --configuration Release
	
#sestaví program pro profiling 
publish_prf:
	cd .\$(prf) && dotnet publish $(publish_opt)

.PHONY: all clean clean_profile test run profile dot_trace_insall setup_profile