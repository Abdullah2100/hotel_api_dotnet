export default class DateFormat {
    public day: string = "";
    public month: string = ""
    public year: string = "";

    constructor(dateHolder: Date) {
        let dateHolderToString = dateHolder.toISOString().split('T')[0];
        this.day = dateHolderToString.split('/')[0] ;
        this.month = dateHolderToString.split('/')[1] ;
        this.year = dateHolderToString.split('/')[2] ;
    }


    public toStringDate() {
        return this.day + '/' + this.month + '/' + this.year
    }

    static toStringDate(dateHolder?: Date | undefined |null) {

        if (dateHolder === undefined || dateHolder===null) return '';
        //let dateHolderToString = dateHolder.toISOString().split('T')[0];
        let dateHolderToString = dateHolder.toString().split('T')[0];

                console.log(`\n\nthis shown the date ${JSON.stringify(dateHolder)}\n\n`)



        let day = dateHolderToString.split('-')[2]
        let month = dateHolderToString.split('-')[1]
        let year = dateHolderToString.split('-')[0]

        return day + '/' + month + '/' + year

    }
}