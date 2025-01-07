import { Button, Heading, Input, Text } from "@chakra-ui/react";
import { useState } from "react";

export const WaitingRoom = ({ joinChat }) => {
	const [userName, setUserName] = useState();
	const [charRoom, setChatRoom] = useState();

	const onSubmit = (e) => {
		e.preventDefault();
		joinChat(userName, charRoom);
	};

	return (
		<form
			onSubmit={onSubmit}
			className="max-w-sm w-full bg-white p-8 rounded shadow-lg"
		>
			<Heading size="lg">Онлайн чат</Heading>
			<div className="mb-4">
				<Text fontSize={"sm"}>Ім'я користувача</Text>
				<Input
					name="username"
					placeholder="Введіть ваше ім'я"
					onChange={(e) => setUserName(e.target.value)}
				/>
			</div>
			<div className="mb-6">
				<Text fontSize={"sm"}>Назва чату</Text>
				<Input
					name="chatname"
					placeholder="Введіть назву чату"
					onChange={(e) => setChatRoom(e.target.value)}
				/>
			</div>
			<Button type="submit" colorScheme="blue">
			Приєднатися
			</Button>
		</form>
	);
};
