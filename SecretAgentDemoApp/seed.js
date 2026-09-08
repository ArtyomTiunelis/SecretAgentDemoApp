db = db.getSiblingDB('promo_app_demo');

db.users.deleteMany({ user_id: { $in: ['USR-5512', 'USR-1001'] } });
db.error_logs.deleteMany({ ticket_id: 'INC-30219' });

db.users.insertMany([
	{
		user_id: 'USR-5512',
		email: 'sam.wilson@company.com',
		is_new_user: true,
		promo_history: null,
		status: 'active'
	},
	{
		user_id: 'USR-1001',
		email: 'jane.doe@company.com',
		is_new_user: false,
		promo_history: ['SPRING10'],
		status: 'active'
	}
]);

db.error_logs.insertOne({
	ticket_id: 'INC-30219',
	user_id: 'USR-5512',
	timestamp: ISODate('2026-09-08T19:10:00Z'),
	endpoint: 'POST /api/v1/promos/redeem',
	status_code: 500,
	message: 'System.NullReferenceException: Object reference not set to an instance of an object.\n   at PromoApp.Api.Controllers.PromoController.RedeemPromo(PromoRedeemRequest request) in /src/Controllers/PromoController.cs:line 35'
});
