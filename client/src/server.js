import { createServer } from "miragejs"
import game from './game'

let game1 = new game()

export default function server() {
    createServer({
        routes() {

            this.get("/api/game", () => {
                return game1.start()
            })

            this.post("/api/game", (schema, request) => {

                let changes;

                try {
                    changes = game1.move(JSON.parse(request.requestBody));
                }
                catch (ex) {
                    console.log(ex)
                }

                return changes
            })

        },
    })
}