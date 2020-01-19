$testProjectLocations = @('test/PaulMiami.AspNetCore.Mvc.Recaptcha.Test')
$outputLocation = 'testResults'
iex ((new-object net.webclient).DownloadString('https://raw.githubusercontent.com/PaulMiami/BuildTools/4cc3b77556ec820f96af8a69ef86959a8abce2c1/Coverage.ps1'))
