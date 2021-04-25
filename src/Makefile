gui  := .\GUI
prf  := .\Profiling
test := .\MathLibTests
doc  := doc\latex

doc_file     := $(doc)\refman.pdf
doc_name     := dokumentace.pdf
archive_name := xkosik10_xkoren11_xkocic02

#cd .. && cd .. == move 2 folders up in path from src

all:gui_build prf_build

gui_build:
	cd $(gui) && dotnet build

prf_build:
	cd $(prf) && dotnet build

pack: pdf clean
	echo "create folders for temp files"
	cd .. && cd .. && mkdir "$(archive_name)\doc"
	cd .. && cd .. && mkdir "$(archive_name)\install"
	cd .. && cd .. && mkdir "$(archive_name)\repo"

	echo "copy required files"
	cd .. && cd .. && xcopy /b /s ".\Team.toString\$(doc_name)"     ".\$(archive_name)\doc"
	cd .. && cd .. && xcopy /b /s ".\Team.toString\install\*"       ".\$(archive_name)\install"
	cd .. && cd .. && xcopy /b /s ".\Team.toString\*"               ".\$(archive_name)\repo"

	echo "create archive"
	cd .. && cd .. && tar -acf $(archive_name).zip $(archive_name)

	echo "delete temp files"
	cd .. && cd .. && rd /s /q "$(archive_name)"

clean:
	echo "delete bin subfolders"
	FOR /d /r . %%d IN (bin) DO @IF EXIST "%%d" rd /s /q "%%d"
	echo "delete obj subfolders"
	FOR /d /r . %%d IN (obj) DO @IF EXIST "%%d" rd /s /q "%%d"
	echo "delete doc folder"
	@IF EXIST "doc" rd /s /q "doc"
	echo "delete test folder"
	cd .. && @IF EXIST "TestResults" rd /s /q "TestResults"

test:
	cd $(test) && dotnet test

doc:
	doxygen

pdf: doc
	echo "generates pdf from latex"
	cd doc\latex && make.bat 1>Nul
	echo "renames pdf and moves pdf to correct folder"
	cd .. && copy ".\src\$(doc_file)" ".\$(doc_name)"

run:
	cd $(gui) && dotnet run

dot_trace_insall:
	dotnet tool install dotnet-trace --global

profile: prf_build
	dotnet-trace collect --show-child-io -- Profiling.exe <numbers10M.txt

.PHONY: all clean test run profile