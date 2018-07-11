rm -r ../deploy

mkdir ../deploy
mkdir ../deploy/.csrun

cp ../bin/*.dll ../deploy/.csrun
cp ../bin/*.exe ../deploy/.csrun
cp ../bin/*.xml ../deploy/.csrun
cp ../bin/*.pdb ../deploy/.csrun
cp ../bin/*.cs ../deploy/.csrun

cp ../bin/*.csrun ../deploy