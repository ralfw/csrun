
rm -r ../release
mkdir ../release
cd ../deploy
rm .DS_Store
zip -r ../release/csrun.zip .
cd ../pipeline